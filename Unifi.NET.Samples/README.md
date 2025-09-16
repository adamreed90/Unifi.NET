# UniFi.NET SDK Samples

This project contains sample applications demonstrating how to use the UniFi.NET SDK libraries.

## UniFi Access Sample

The UniFi Access sample is a console application that demonstrates key features of the UniFi Access SDK including:

- User management (create, list, update)
- NFC card enrollment and assignment
- PIN code generation and assignment
- Device discovery and management

### Prerequisites

1. **UniFi Access Console**: You need access to a UniFi Access console (v1.9.1 or later)
2. **API Token**: Generate an API token from your UniFi Access console
3. **.NET 9 SDK**: Install the .NET 9 SDK from [dotnet.microsoft.com](https://dotnet.microsoft.com)

### Getting Started

1. Build the project:
```bash
~/.dotnet/dotnet build Unifi.NET.Samples.csproj
```

2. Run the sample:
```bash
~/.dotnet/dotnet run
```

3. When prompted, enter:
   - Your UniFi Access console URL (e.g., `https://192.168.1.100:12445`)
   - Your API token

### Features

The sample provides a comprehensive menu demonstrating all major SDK capabilities:

#### User Management
- **Create New User** - Add users with name, email, and employee ID
- **List All Users** - View all users with their status and credentials
- **Search Users** - Find users by name or keyword
- **View User Details** - Get complete information about a specific user

#### User Groups
- **List User Groups** - View all groups with hierarchy information
- **Create User Group** - Create new groups with optional parent groups
- **Add Users to Group** - Bulk assign users to groups
- **View Users in Group** - See group membership

#### NFC Card Management
- **Register NFC Card** - Interactive enrollment with device selection
- **List NFC Cards** - View all cards in the system
- **Import NFC Cards** - Bulk import from CSV file
- **Direct Card Assignment** - Assign existing cards to users with force reassignment

#### PIN Code Management
- **Assign PIN Code** - Generate or specify custom PINs (4-8 digits)
- **View PIN Status** - Check if users have PINs assigned

#### Access Policies
- **List Access Policies** - View all policies with resources
- **Create Access Policy** - Define new policies with schedules
- **Assign to Users** - Grant access to specific users

#### Door Management
- **List Doors** - View all doors with status
- **Unlock Door** - Remote unlock for testing

#### Device Management
- **List Devices** - View all UniFi Access devices with details

#### Reporting
- **Export Last Activity Report (CSV)** - Generate CSV report of all users' last door access activity, sorted from oldest to newest

### Native AOT Compilation

This sample fully supports Native AOT compilation for optimal performance and minimal memory footprint:

```bash
# Publish for Linux
~/.dotnet/dotnet publish -c Release -r linux-x64

# Publish for Windows
~/.dotnet/dotnet publish -c Release -r win-x64

# Publish for macOS (Apple Silicon)
~/.dotnet/dotnet publish -c Release -r osx-arm64
```

**Key Features:**
- Zero runtime reflection - all JSON serialization uses source generators
- ASP.NET Core-style JsonTypeInfoResolver pattern for type resolution
- Service-specific JsonSerializerContext classes for optimal organization
- No AOT warnings - fully trimming and AOT compatible
- Typical binary size: ~15-20MB (compared to ~70MB+ without AOT)

### Security Notes

- The sample disables SSL certificate validation by default for development
- In production, set `ValidateSslCertificate = true`
- Store API tokens securely (use environment variables or secure configuration)
- Never commit API tokens to source control

### Troubleshooting

#### Connection Issues
- Verify the console URL is correct and accessible
- Check that the API token is valid
- Ensure port 12445 is not blocked by firewall

#### NFC Card Enrollment
- Ensure the selected device supports NFC (UAH or UAHP models)
- Card must be tapped within 60 seconds of session creation
- Only one enrollment session can be active at a time

#### API Errors
- Check console logs for detailed error messages
- Verify user has appropriate permissions
- Ensure UniFi Access version is 1.9.1 or later

### Example Output

```
UniFi Access SDK Sample - User and NFC Card Management
========================================================

Enter UniFi Access Console URL (e.g., https://192.168.1.100:12445): https://192.168.1.100:12445
Enter UniFi Access API Token: ********

--- Main Menu ---
1. Create new user
2. List all users
3. Register NFC card for user
4. Assign PIN code to user
5. List devices
6. Exit

Select option: 1

--- Create New User ---
First Name: John
Last Name: Smith
Email: john.smith@example.com
Employee ID (optional): EMP001

✓ User created successfully!
  User ID: abc123def456
  Name: John Smith
  Email: john.smith@example.com
  Status: ACTIVE
```

## Additional Samples

Additional samples for UniFi Network, UniFi Protect, and UniFi Site Manager will be added as those SDKs are developed.

## Contributing

See the main [CONTRIBUTING.md](../CONTRIBUTING.md) for guidelines on contributing to this project.

## License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.