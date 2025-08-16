#!/usr/bin/env python3
"""
PDF to Markdown Converter for API Documentation
Specifically designed for UniFi Access API documentation conversion.
Can be adapted for other technical PDF documents.

Usage:
    1. First extract text from PDF: pdftotext -layout input.pdf output.txt
    2. Then run: python3 pdf_to_markdown_converter.py output.txt final.md
"""

import re
import sys
from typing import Optional, List, Tuple

class APIDocMarkdownConverter:
    def __init__(self):
        self.in_code_block = False
        self.code_buffer = []
        self.in_table = False
        self.table_buffer = []
        self.current_section = None
        
    def detect_section_type(self, line: str) -> Optional[str]:
        """Detect the type of section based on patterns"""
        if re.match(r'^(\d+)\.\s+[A-Z]', line):
            return 'main_section'
        elif re.match(r'^\s*(\d+\.\d+)\s+[A-Z]', line):
            return 'subsection'
        elif 'Request URL:' in line:
            return 'api_endpoint'
        elif line.strip() in ['Request Header', 'Request Body', 'Response Body', 
                              'Request Path', 'Request Sample', 'Response Sample']:
            return 'api_section'
        elif re.search(r'Parameter\s+Required\s+Type', line):
            return 'table_header'
        return None
    
    def clean_table_of_contents_line(self, line: str) -> Optional[str]:
        """Clean TOC lines removing dots and page numbers"""
        # Match TOC pattern: "1.1 Title .....page"
        match = re.match(r'^(\s*)(\d+(?:\.\d+)?)\s+(.+?)\.+\d+$', line)
        if match:
            indent = match.group(1)
            number = match.group(2)
            title = match.group(3).strip()
            # Create proper markdown link
            level = '  ' * (len(indent) // 4)  # Convert indent to markdown list levels
            anchor = title.lower().replace(' ', '-').replace('/', '-').replace('&', 'and')
            return f"{level}- [{number} {title}](#{number.replace('.', '')}-{anchor})"
        return None
    
    def format_api_endpoint(self, lines: List[str], start_idx: int) -> Tuple[List[str], int]:
        """Format an API endpoint section"""
        output = []
        i = start_idx
        
        # Find all the API metadata
        url = None
        permission = None
        method = None
        requirements = None
        description_lines = []
        
        while i < len(lines) and i < start_idx + 20:
            line = lines[i].strip()
            
            if 'Request URL:' in line:
                url = line.split('Request URL:')[1].strip()
            elif 'Permission Key:' in line:
                permission = line.split('Permission Key:')[1].strip()
            elif re.match(r'^Method:\s+\w+$', line):
                method = line.split('Method:')[1].strip()
            elif 'UniFi Access Requirement:' in line:
                requirements = line.split('UniFi Access Requirement:')[1].strip()
            elif line and not any(x in line for x in ['Request', 'Permission', 'Method', 'UniFi']):
                # This might be a description
                if not re.match(r'^(Request|Response)', line):
                    description_lines.append(line)
            
            # Stop when we hit the next section
            if line in ['Request Header', 'Request Body', 'Request Path', 'Response Body']:
                break
            
            i += 1
        
        # Format the endpoint info as a nice table
        if url:
            output.append("\n### Endpoint Information\n")
            output.append(f"- **URL:** `{url}`\n")
            if method:
                output.append(f"- **Method:** `{method}`\n")
            if permission:
                output.append(f"- **Permission:** `{permission}`\n")
            if requirements:
                output.append(f"- **Requirements:** {requirements}\n")
            if description_lines:
                output.append(f"- **Description:** {' '.join(description_lines)}\n")
            output.append("\n")
        
        return output, i
    
    def format_parameter_table(self, lines: List[str], start_idx: int) -> Tuple[List[str], int]:
        """Format a parameter table"""
        output = []
        i = start_idx
        
        # Parse the header
        header_line = lines[i].strip()
        headers = re.split(r'\s{2,}', header_line)
        headers = [h.strip() for h in headers if h.strip()]
        
        if headers:
            output.append("\n| " + " | ".join(headers) + " |\n")
            output.append("|" + "---|" * len(headers) + "\n")
            
            i += 1
            
            # Parse table rows
            while i < len(lines):
                line = lines[i].strip()
                
                # Check if we've hit the end of the table
                if not line or re.match(r'^(Request|Response|Note:)', line):
                    break
                
                # Parse table row
                cells = re.split(r'\s{2,}', line)
                cells = [c.strip() for c in cells if c.strip()]
                
                if len(cells) >= 2:  # Must have at least 2 cells to be valid
                    # Pad cells if needed
                    while len(cells) < len(headers):
                        cells.append("")
                    
                    # Format cells - wrap code-like content in backticks
                    formatted_cells = []
                    for j, cell in enumerate(cells):
                        if j == 0 and '_' in cell:  # Parameter names
                            formatted_cells.append(f"`{cell}`")
                        elif cell in ['T', 'F']:  # Required field
                            formatted_cells.append('✓' if cell == 'T' else '✗')
                        elif cell in ['String', 'Integer', 'Boolean', 'Array', 'Object', 
                                     'Array[String]', 'Array[Object]']:
                            formatted_cells.append(f"`{cell}`")
                        else:
                            formatted_cells.append(cell)
                    
                    output.append("| " + " | ".join(formatted_cells[:len(headers)]) + " |\n")
                
                i += 1
        
        return output, i
    
    def format_code_block(self, lines: List[str], start_idx: int) -> Tuple[List[str], int]:
        """Format a code block (curl command or JSON)"""
        output = []
        i = start_idx
        code_lines = []
        
        # Determine code type
        first_line = lines[i].strip()
        # Remove line numbers if present
        first_line_clean = re.sub(r'^\d+\s+', '', first_line)
        
        if 'curl' in first_line_clean:
            lang = 'bash'
        elif first_line_clean.startswith('{') or first_line_clean.startswith('['):
            lang = 'json'
        else:
            lang = 'text'
        
        output.append(f"\n```{lang}\n")
        
        # Collect code lines
        while i < len(lines):
            line = lines[i]
            
            # Remove line numbers
            clean_line = re.sub(r'^\s*\d+\s+', '', line)
            
            # Check for end of code block
            if i > start_idx and clean_line.strip() == '' and i + 1 < len(lines):
                next_line = lines[i + 1].strip()
                if next_line and not re.match(r'^\s*\d+\s+', lines[i + 1]):
                    # End of code block
                    break
            
            code_lines.append(clean_line)
            i += 1
            
            # Also check for obvious end markers
            if clean_line.strip() in ['Request Sample', 'Response Sample', 'Request Body', 
                                      'Response Body', 'Request Header']:
                i -= 1  # Back up one line
                break
        
        output.extend(code_lines)
        output.append("```\n\n")
        
        return output, i
    
    def convert_file(self, input_file: str, output_file: str):
        """Main conversion function"""
        with open(input_file, 'r', encoding='utf-8') as f:
            lines = f.readlines()
        
        output = []
        
        # Add header
        output.append("# UniFi Access API Reference\n\n")
        output.append("> Complete API documentation for UniFi Access\n\n")
        output.append("**Version:** 3.3.21  \n")
        output.append("**Generated from:** Official UniFi Access API Documentation\n\n")
        output.append("---\n\n")
        
        # Generate Table of Contents
        output.append("## Table of Contents\n\n")
        
        in_toc = True
        toc_end = 0
        
        for i, line in enumerate(lines[:250]):
            if in_toc:
                toc_line = self.clean_table_of_contents_line(line)
                if toc_line:
                    output.append(toc_line + "\n")
                elif "1. Introduction" in line and i > 100:
                    # We've hit the actual content
                    in_toc = False
                    toc_end = i
                    break
        
        output.append("\n---\n\n")
        
        # Process main content
        i = toc_end if toc_end > 0 else 200
        
        while i < len(lines):
            line = lines[i]
            
            # Skip empty lines
            if not line.strip():
                if not self.in_code_block:
                    output.append("\n")
                i += 1
                continue
            
            # Main section headers
            if re.match(r'^(\d+)\.\s+([A-Z].+)$', line.strip()):
                match = re.match(r'^(\d+)\.\s+(.+)$', line.strip())
                section_num = match.group(1)
                section_title = match.group(2).rstrip('.').strip()
                output.append(f"\n# {section_num}. {section_title}\n\n")
                i += 1
                continue
            
            # Subsection headers
            if re.match(r'^(\d+\.\d+)\s+(.+)$', line.strip()) and 'Request URL:' not in line:
                match = re.match(r'^(\d+\.\d+)\s+(.+)$', line.strip())
                if match:
                    section_num = match.group(1)
                    section_title = match.group(2).rstrip('.').strip()
                    output.append(f"\n## {section_num} {section_title}\n\n")
                    i += 1
                    continue
            
            # API endpoint section
            if 'Request URL:' in line:
                endpoint_output, new_i = self.format_api_endpoint(lines, i)
                output.extend(endpoint_output)
                i = new_i
                continue
            
            # API section headers
            if line.strip() in ['Request Header', 'Request Body', 'Response Body', 
                               'Request Path', 'Request Sample', 'Response Sample']:
                output.append(f"\n### {line.strip()}\n\n")
                i += 1
                continue
            
            # Parameter tables
            if 'Parameter' in line and any(x in line for x in ['Required', 'Type', 'Description']):
                table_output, new_i = self.format_parameter_table(lines, i)
                output.extend(table_output)
                i = new_i
                continue
            
            # Code blocks
            if (re.match(r'^\s*\d+\s+(curl|{|\[)', line) or 
                line.strip().startswith('curl ') or 
                (line.strip().startswith('{') and '"code"' in line)):
                code_output, new_i = self.format_code_block(lines, i)
                output.extend(code_output)
                i = new_i
                continue
            
            # HTTP status codes
            if re.match(r'^\s*\d{3}\s+\w+', line.strip()):
                parts = re.split(r'\s{2,}', line.strip())
                if len(parts) >= 2:
                    output.append(f"- **{parts[0]}** - {' '.join(parts[1:])}\n")
                i += 1
                continue
            
            # Error codes
            if re.match(r'^\s*CODE_\w+', line.strip()):
                parts = re.split(r'\s{2,}', line.strip())
                if len(parts) >= 2:
                    output.append(f"- `{parts[0]}` - {' '.join(parts[1:])}\n")
                i += 1
                continue
            
            # Default: add the line as-is
            output.append(line)
            i += 1
        
        # Post-process to fix common issues
        content = ''.join(output)
        
        # Fix numbered lists that were incorrectly converted to headers
        content = re.sub(r'^# (\d+)\. (.+)$', r'\1. \2', content, flags=re.MULTILINE)
        
        # Fix bullet points in numbered lists
        content = re.sub(r'^  (\d+)\. ', r'   \1. ', content, flags=re.MULTILINE)
        
        # Clean up extra blank lines
        content = re.sub(r'\n{3,}', '\n\n', content)
        
        # Write output
        with open(output_file, 'w', encoding='utf-8') as f:
            f.write(content)
        
        print(f"✅ Conversion complete! Output saved to {output_file}")
        return output_file

def main():
    if len(sys.argv) < 3:
        print("Usage: python3 pdf_to_markdown_converter.py <input.txt> <output.md>")
        print("\nFirst extract text from PDF:")
        print("  pdftotext -layout input.pdf output.txt")
        print("\nThen convert to markdown:")
        print("  python3 pdf_to_markdown_converter.py output.txt final.md")
        sys.exit(1)
    
    input_file = sys.argv[1]
    output_file = sys.argv[2]
    
    converter = APIDocMarkdownConverter()
    converter.convert_file(input_file, output_file)

if __name__ == "__main__":
    main()