using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Unifi.NET.Access;
using Unifi.NET.Access.Configuration;
using Unifi.NET.Access.Exceptions;
using Unifi.NET.Access.Models.Credentials;
using Unifi.NET.Access.Models.Users;
using Unifi.NET.Access.Models.UserGroups;
using Unifi.NET.Access.Models.Devices;
using Unifi.NET.Access.Models.Doors;
using Unifi.NET.Access.Models.SystemLogs;
using System.Text;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
    })
    .Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("UniFi.Access.Sample");

try
{
    Console.WriteLine("UniFi Access SDK Sample - Complete Access Management");
    Console.WriteLine("=====================================================\n");

    // Configuration
    Console.Write("Enter UniFi Access Console URL (e.g., https://192.168.1.100:12445): ");
    var baseUrl = Console.ReadLine()?.Trim();
    
    Console.Write("Enter UniFi Access API Token: ");
    var apiToken = Console.ReadLine()?.Trim();
    
    if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiToken))
    {
        Console.WriteLine("Error: Console URL and API Token are required.");
        return 1;
    }

    var config = new UnifiAccessConfiguration
    {
        BaseUrl = baseUrl,
        ApiToken = apiToken,
        ValidateSslCertificate = false, // Set to true in production
        TimeoutSeconds = 30
    };

    using var client = new UnifiAccessClient(config);

    // Menu loop
    bool exit = false;
    while (!exit)
    {
        Console.WriteLine("\n=== Main Menu ===");
        Console.WriteLine("\n-- User Management --");
        Console.WriteLine("1. Create new user");
        Console.WriteLine("2. List all users");
        Console.WriteLine("3. Search users");
        
        Console.WriteLine("\n-- User Group Management --");
        Console.WriteLine("4. Create user group");
        Console.WriteLine("5. List user groups");
        Console.WriteLine("6. Add users to group");
        Console.WriteLine("7. View users in group");
        
        Console.WriteLine("\n-- NFC Card Management --");
        Console.WriteLine("8. Register NFC card (enrollment)");
        Console.WriteLine("9. Import NFC cards from CSV");
        Console.WriteLine("10. List all NFC cards");
        Console.WriteLine("11. Assign existing NFC card to user");
        Console.WriteLine("12. Remove NFC card from user");
        
        Console.WriteLine("\n-- PIN Management --");
        Console.WriteLine("13. Assign PIN code to user");
        
        Console.WriteLine("\n-- Workflow Examples --");
        Console.WriteLine("14. Complete onboarding workflow");
        Console.WriteLine("15. Bulk import users and cards");
        
        Console.WriteLine("\n-- System --");
        Console.WriteLine("16. List devices");
        Console.WriteLine("17. Export Last Activity Report (CSV)");
        Console.WriteLine("18. Exit");
        Console.Write("\nSelect option: ");
        
        var choice = Console.ReadLine()?.Trim();
        
        switch (choice)
        {
            case "1":
                await CreateNewUser(client, logger);
                break;
            case "2":
                await ListUsers(client, logger);
                break;
            case "3":
                await SearchUsers(client, logger);
                break;
            case "4":
                await CreateUserGroup(client, logger);
                break;
            case "5":
                await ListUserGroups(client, logger);
                break;
            case "6":
                await AddUsersToGroup(client, logger);
                break;
            case "7":
                await ViewUsersInGroup(client, logger);
                break;
            case "8":
                await RegisterNfcCard(client, logger);
                break;
            case "9":
                await ImportNfcCards(client, logger);
                break;
            case "10":
                await ListNfcCards(client, logger);
                break;
            case "11":
                await AssignExistingNfcCard(client, logger);
                break;
            case "12":
                await RemoveNfcCardFromUser(client, logger);
                break;
            case "13":
                await AssignPinCode(client, logger);
                break;
            case "14":
                await CompleteOnboardingWorkflow(client, logger);
                break;
            case "15":
                await BulkImportUsersAndCards(client, logger);
                break;
            case "16":
                await ListDevices(client, logger);
                break;
            case "17":
                await ExportLastActivityReport(client, logger);
                break;
            case "18":
                exit = true;
                break;
            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred");
    Console.WriteLine($"\nError: {ex.Message}");
    return 1;
}

Console.WriteLine("\nGoodbye!");
return 0;

async Task CreateNewUser(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n--- Create New User ---");
    
    Console.Write("First Name: ");
    var firstName = Console.ReadLine()?.Trim();
    
    Console.Write("Last Name: ");
    var lastName = Console.ReadLine()?.Trim();
    
    Console.Write("Email: ");
    var email = Console.ReadLine()?.Trim();
    
    Console.Write("Employee ID (optional): ");
    var employeeId = Console.ReadLine()?.Trim();
    
    if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
    {
        Console.WriteLine("Error: First and last name are required.");
        return;
    }

    try
    {
        var request = new CreateUserRequest
        {
            FirstName = firstName,
            LastName = lastName,
            UserEmail = email,
            EmployeeNumber = string.IsNullOrWhiteSpace(employeeId) ? null : employeeId
        };

        logger.LogInformation("Creating user {FirstName} {LastName}...", firstName, lastName);
        var user = await client.Users.CreateUserAsync(request);
        
        Console.WriteLine($"\n✓ User created successfully!");
        Console.WriteLine($"  User ID: {user.Id}");
        Console.WriteLine($"  Name: {user.FirstName} {user.LastName}");
        Console.WriteLine($"  Email: {user.UserEmail}");
        Console.WriteLine($"  Status: {user.Status}");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to create user");
        Console.WriteLine($"Error creating user: {ex.Message}");
    }
}

async Task ListUsers(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n--- List All Users ---");
    
    try
    {
        logger.LogInformation("Fetching users...");
        var users = await client.Users.GetUsersAsync();
        
        if (!users.Any())
        {
            Console.WriteLine("No users found.");
            return;
        }

        Console.WriteLine($"\nFound {users.Count()} users:\n");
        
        foreach (var user in users)
        {
            Console.WriteLine($"• {user.FirstName} {user.LastName}");
            Console.WriteLine($"  ID: {user.Id}");
            Console.WriteLine($"  Email: {user.UserEmail ?? "N/A"}");
            Console.WriteLine($"  Status: {user.Status}");
            Console.WriteLine($"  NFC Cards: {user.NfcCards?.Count ?? 0}");
            Console.WriteLine($"  Has PIN: {user.PinCode != null}");
            Console.WriteLine();
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to list users");
        Console.WriteLine($"Error listing users: {ex.Message}");
    }
}

async Task SearchUsers(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n--- Search Users ---");
    
    Console.Write("Enter search keyword: ");
    var keyword = Console.ReadLine()?.Trim();
    
    if (string.IsNullOrWhiteSpace(keyword))
    {
        Console.WriteLine("Error: Search keyword is required.");
        return;
    }

    try
    {
        logger.LogInformation("Searching users with keyword '{Keyword}'...", keyword);
        var users = await client.Users.SearchUsersAsync(keyword);
        
        if (!users.Any())
        {
            Console.WriteLine($"No users found matching '{keyword}'.");
            return;
        }

        Console.WriteLine($"\nFound {users.Count()} matching users:\n");
        
        foreach (var user in users)
        {
            Console.WriteLine($"• {user.FirstName} {user.LastName}");
            Console.WriteLine($"  ID: {user.Id}");
            Console.WriteLine($"  Email: {user.UserEmail ?? "N/A"}");
            Console.WriteLine();
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to search users");
        Console.WriteLine($"Error searching users: {ex.Message}");
    }
}

async Task CreateUserGroup(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n--- Create User Group ---");
    
    Console.Write("Group Name: ");
    var name = Console.ReadLine()?.Trim();
    
    Console.Write("Description (optional): ");
    var description = Console.ReadLine()?.Trim();
    
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Error: Group name is required.");
        return;
    }

    try
    {
        var request = new UserGroupRequest
        {
            Name = name,
            Description = string.IsNullOrWhiteSpace(description) ? null : description
        };

        logger.LogInformation("Creating user group '{Name}'...", name);
        var group = await client.UserGroups.CreateUserGroupAsync(request);
        
        Console.WriteLine($"\n✓ User group created successfully!");
        Console.WriteLine($"  Group ID: {group.Id}");
        Console.WriteLine($"  Name: {group.Name}");
        Console.WriteLine($"  Description: {group.Description ?? "N/A"}");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to create user group");
        Console.WriteLine($"Error creating user group: {ex.Message}");
    }
}

async Task ListUserGroups(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n--- List User Groups ---");
    
    try
    {
        logger.LogInformation("Fetching user groups...");
        var groupsResponse = await client.UserGroups.GetUserGroupsAsync();
        var groups = groupsResponse.Items;

        if (!groups.Any())
        {
            Console.WriteLine("No user groups found.");
            return;
        }

        Console.WriteLine($"\nFound {groups.Count} user groups (Page {groupsResponse.Page} of {groupsResponse.TotalPages}):\n");

        foreach (var group in groups)
        {
            Console.WriteLine($"• {group.Name}");
            Console.WriteLine($"  ID: {group.Id}");
            Console.WriteLine($"  Description: {group.Description ?? "N/A"}");
            if (group.ParentId != null)
            {
                Console.WriteLine($"  Parent ID: {group.ParentId}");
            }
            Console.WriteLine($"  Is System: {group.IsSystem}");
            Console.WriteLine();
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to list user groups");
        Console.WriteLine($"Error listing user groups: {ex.Message}");
    }
}

async Task AddUsersToGroup(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n--- Add Users to Group ---");
    
    // List groups
    Console.Write("Enter Group ID (or press Enter to list groups): ");
    var groupId = Console.ReadLine()?.Trim();
    
    if (string.IsNullOrWhiteSpace(groupId))
    {
        await ListUserGroups(client, logger);
        Console.Write("\nEnter Group ID: ");
        groupId = Console.ReadLine()?.Trim();
    }
    
    if (string.IsNullOrWhiteSpace(groupId))
    {
        Console.WriteLine("Error: Group ID is required.");
        return;
    }

    // Get user IDs
    Console.WriteLine("\nEnter User IDs to add (comma-separated):");
    Console.WriteLine("Or press Enter to list users first:");
    var userIdsInput = Console.ReadLine()?.Trim();
    
    if (string.IsNullOrWhiteSpace(userIdsInput))
    {
        await ListUsers(client, logger);
        Console.WriteLine("\nEnter User IDs to add (comma-separated):");
        userIdsInput = Console.ReadLine()?.Trim();
    }
    
    if (string.IsNullOrWhiteSpace(userIdsInput))
    {
        Console.WriteLine("Error: At least one User ID is required.");
        return;
    }

    var userIds = userIdsInput.Split(',').Select(id => id.Trim()).Where(id => !string.IsNullOrWhiteSpace(id)).ToList();

    try
    {
        var request = new AssignUsersToGroupRequest
        {
            UserIds = userIds
        };

        logger.LogInformation("Adding {Count} users to group {GroupId}...", userIds.Count, groupId);
        await client.UserGroups.AssignUsersToGroupAsync(groupId, request);
        
        Console.WriteLine($"\n✓ Successfully added {userIds.Count} user(s) to group!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to add users to group");
        Console.WriteLine($"Error adding users to group: {ex.Message}");
    }
}

async Task ViewUsersInGroup(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n--- View Users in Group ---");
    
    Console.Write("Enter Group ID (or press Enter to list groups): ");
    var groupId = Console.ReadLine()?.Trim();
    
    if (string.IsNullOrWhiteSpace(groupId))
    {
        await ListUserGroups(client, logger);
        Console.Write("\nEnter Group ID: ");
        groupId = Console.ReadLine()?.Trim();
    }
    
    if (string.IsNullOrWhiteSpace(groupId))
    {
        Console.WriteLine("Error: Group ID is required.");
        return;
    }

    try
    {
        logger.LogInformation("Fetching users in group {GroupId}...", groupId);
        var usersResponse = await client.UserGroups.GetUsersInGroupAsync(groupId);
        var users = usersResponse.Items;

        if (!users.Any())
        {
            Console.WriteLine("No users found in this group.");
            return;
        }

        Console.WriteLine($"\nFound {users.Count} users in group (Page {usersResponse.Page} of {usersResponse.TotalPages}):\n");

        foreach (var user in users)
        {
            Console.WriteLine($"• {user.FirstName} {user.LastName}");
            Console.WriteLine($"  ID: {user.Id}");
            Console.WriteLine($"  Email: {user.UserEmail ?? "N/A"}");
            Console.WriteLine();
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to get users in group");
        Console.WriteLine($"Error getting users in group: {ex.Message}");
    }
}

async Task RegisterNfcCardForUser(IUnifiAccessClient client, ILogger logger, string userId, string firstName, string lastName)
{
    Console.WriteLine($"\n🔐 Registering NFC card for {firstName} {lastName}...");
    await RegisterNfcCardCore(client, logger, userId, $"{firstName} {lastName}");
}

async Task RegisterNfcCard(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n--- Register NFC Card via Enrollment ---");
    
    Console.WriteLine("\nSelect user for NFC card assignment:");
    Console.WriteLine("1. Enter User ID directly");
    Console.WriteLine("2. Search for user by name");
    Console.WriteLine("3. List all users");
    Console.Write("\nSelect option (1-3): ");
    
    var option = Console.ReadLine()?.Trim();
    string? userId = null;
    
    switch (option)
    {
        case "1":
            Console.Write("Enter User ID: ");
            userId = Console.ReadLine()?.Trim();
            break;
            
        case "2":
            Console.Write("Enter search keyword (name/email): ");
            var keyword = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                logger.LogInformation("Searching for users...");
                var searchResults = await client.Users.SearchUsersAsync(keyword);
                
                if (!searchResults.Any())
                {
                    Console.WriteLine("No users found matching the search criteria.");
                    return;
                }
                
                Console.WriteLine($"\nFound {searchResults.Count()} user(s):\n");
                var userList = searchResults.ToList();
                for (int i = 0; i < userList.Count; i++)
                {
                    var user = userList[i];
                    Console.WriteLine($"{i + 1}. {user.FirstName} {user.LastName}");
                    Console.WriteLine($"   ID: {user.Id}");
                    Console.WriteLine($"   Email: {user.UserEmail ?? "N/A"}");
                    Console.WriteLine($"   Employee #: {user.EmployeeNumber ?? "N/A"}");
                }
                
                Console.Write("\nSelect user number: ");
                if (int.TryParse(Console.ReadLine(), out int userIndex) && userIndex > 0 && userIndex <= userList.Count)
                {
                    userId = userList[userIndex - 1].Id;
                }
            }
            break;
            
        case "3":
            await ListUsers(client, logger);
            Console.Write("\nEnter User ID from the list: ");
            userId = Console.ReadLine()?.Trim();
            break;
            
        default:
            Console.WriteLine("Invalid option.");
            return;
    }
    
    if (string.IsNullOrWhiteSpace(userId))
    {
        Console.WriteLine("Error: User ID is required.");
        return;
    }

    await RegisterNfcCardCore(client, logger, userId, null);
}

async Task RegisterNfcCardCore(IUnifiAccessClient client, ILogger logger, string userId, string? userName)
{
    // Get available devices and doors
    logger.LogInformation("Fetching available devices and doors...");
    var devicesTask = client.Devices.GetDevicesAsync();
    var doorsTask = client.Doors.GetDoorsAsync();
    
    await Task.WhenAll(devicesTask, doorsTask);
    
    var devices = await devicesTask;
    var doors = await doorsTask;
    
    // Create door lookup
    var doorLookup = doors.ToDictionary(d => d.Id, d => d);
    
    // Filter to only NFC-capable readers (not hubs)
    var nfcCapableTypes = new[] { "UA-G3", "UA-G2-PRO", "UA-G2-MINI", "UDA-LITE" };
    var readers = devices.Where(d => nfcCapableTypes.Contains(d.Type)).ToList();
    
    if (!readers.Any())
    {
        Console.WriteLine("Error: No NFC-capable readers found.");
        Console.WriteLine("\nAll devices found:");
        foreach (var d in devices)
        {
            Console.WriteLine($"  - {d.Name} ({d.Type})");
        }
        return;
    }

    // Group readers by location (door)
    var readersByLocation = readers
        .GroupBy(r => r.LocationId)
        .OrderBy(g => doorLookup.ContainsKey(g.Key) ? doorLookup[g.Key].FullName : "Unknown")
        .ToList();
    
    Console.WriteLine("\nAvailable NFC Readers:");
    Console.WriteLine("══════════════════════");
    
    var readerIndex = 1;
    var indexedReaders = new List<DeviceResponse>();
    
    foreach (var locationGroup in readersByLocation)
    {
        var locationId = locationGroup.Key;
        DoorResponse? door = null;
        doorLookup.TryGetValue(locationId, out door);
        
        // Display door information
        if (door != null)
        {
            Console.WriteLine($"\n📍 Door: {door.Name}");
            Console.WriteLine($"   Full Path: {door.FullName}");
            
            // Show door status if available
            if (!string.IsNullOrWhiteSpace(door.DoorLockRelayStatus))
            {
                var lockIcon = door.DoorLockRelayStatus == "unlock" ? "🔓" : "🔒";
                var positionIcon = door.DoorPositionStatus switch
                {
                    "open" => "🚪",
                    "close" => "🚪",
                    _ => "❓"
                };
                Console.WriteLine($"   Status: {lockIcon} {door.DoorLockRelayStatus} | {positionIcon} {door.DoorPositionStatus ?? "no sensor"}");
            }
        }
        else
        {
            Console.WriteLine($"\n📍 Location: Unknown (ID: {locationId})");
        }
        
        // List readers at this location
        foreach (var device in locationGroup)
        {
            var displayName = !string.IsNullOrWhiteSpace(device.Alias) ? device.Alias : device.Name;
            
            // Show parent hub info if connected
            var hubInfo = "";
            if (!string.IsNullOrWhiteSpace(device.ConnectedUahId))
            {
                var hub = devices.FirstOrDefault(d => d.Id == device.ConnectedUahId);
                if (hub != null)
                {
                    var hubName = !string.IsNullOrWhiteSpace(hub.Alias) ? hub.Alias : hub.Name;
                    hubInfo = $" → Hub: {hubName}";
                }
                else
                {
                    hubInfo = $" → Hub: ...{device.ConnectedUahId.Substring(Math.Max(0, device.ConnectedUahId.Length - 4))}";
                }
            }
            else
            {
                hubInfo = " [Standalone]";
            }
            
            Console.WriteLine($"\n  [{readerIndex}] {displayName}");
            Console.WriteLine($"      Model: {device.Name} | Type: {device.Type}");
            Console.WriteLine($"      ID: {device.Id}{hubInfo}");
            
            indexedReaders.Add(device);
            readerIndex++;
        }
    }
    
    Console.WriteLine("\n──────────────────────");
    Console.Write("\nSelect reader number: ");
    if (!int.TryParse(Console.ReadLine(), out int deviceIndex) || deviceIndex < 1 || deviceIndex > indexedReaders.Count)
    {
        Console.WriteLine("Invalid reader selection.");
        return;
    }
    
    var selectedDevice = indexedReaders[deviceIndex - 1];

    try
    {
        // Ask about resetting cards from other sites
        Console.Write("\nAllow resetting cards enrolled at other sites? (Y/N): ");
        var allowReset = Console.ReadLine()?.Trim()?.ToUpperInvariant() == "Y";
        
        // Create enrollment session
        logger.LogInformation("Creating NFC enrollment session...");
        logger.LogInformation("Using device: {DeviceName} (ID: {DeviceId}, Type: {DeviceType})", 
            selectedDevice.Name, selectedDevice.Id, selectedDevice.Type);
        
        // Validate device ID format (should be 12 hex characters)
        if (selectedDevice.Id.Length != 12 || !System.Text.RegularExpressions.Regex.IsMatch(selectedDevice.Id, "^[0-9a-fA-F]{12}$"))
        {
            logger.LogWarning("Device ID format may be incorrect. Expected 12 hex characters, got: {DeviceId}", selectedDevice.Id);
        }
        
        var sessionRequest = new CreateNfcEnrollmentSessionRequest
        {
            DeviceId = selectedDevice.Id,
            ResetUaCard = allowReset
        };
        
        var session = await client.Credentials.CreateNfcEnrollmentSessionAsync(sessionRequest);
        Console.WriteLine($"\n✓ Enrollment session created!");
        Console.WriteLine($"  Session ID: {session.SessionId}");
        Console.WriteLine($"\n📍 Device: {selectedDevice.Name}");
        Console.WriteLine("\n⏳ Instructions:");
        Console.WriteLine("   1. Place your NFC card on the reader");
        Console.WriteLine("   2. HOLD the card for at least 5 seconds");
        Console.WriteLine("   3. Wait for confirmation beep/light");
        Console.WriteLine("\n   Timeout: 60 seconds | Press 'C' to cancel | Press 'R' to retry\n");

        // Poll for card detection
        var startTime = DateTime.UtcNow;
        var timeout = TimeSpan.FromSeconds(60);
        string? detectedCardId = null;
        bool cardAlreadyEnrolled = false;
        int retryCount = 0;
        const int maxRetries = 3;
        
        while (DateTime.UtcNow - startTime < timeout)
        {
            // Check for user input
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);
                if (key.KeyChar == 'c' || key.KeyChar == 'C')
                {
                    Console.WriteLine("\n\n⚠️  Cancelling enrollment session...");
                    await client.Credentials.CancelNfcEnrollmentSessionAsync(session.SessionId);
                    Console.WriteLine("✓ Enrollment cancelled.");
                    return;
                }
                else if (key.KeyChar == 'r' || key.KeyChar == 'R')
                {
                    Console.WriteLine("\n\n🔄 Restarting enrollment session...");
                    await client.Credentials.CancelNfcEnrollmentSessionAsync(session.SessionId);
                    
                    // Create new session
                    session = await client.Credentials.CreateNfcEnrollmentSessionAsync(sessionRequest);
                    Console.WriteLine($"✓ New session created: {session.SessionId}");
                    Console.WriteLine("📍 Please tap and HOLD your card on the reader for 5 seconds...\n");
                    
                    // Reset timer
                    startTime = DateTime.UtcNow;
                    detectedCardId = null;
                    cardAlreadyEnrolled = false;
                    retryCount++;
                    
                    if (retryCount >= maxRetries)
                    {
                        Console.WriteLine($"⚠️  Maximum retries ({maxRetries}) reached. Please check the card and reader.");
                    }
                }
            }
            
            await Task.Delay(2000); // Poll every 2 seconds
            
            try
            {
                var status = await client.Credentials.GetNfcEnrollmentStatusAsync(session.SessionId);
                
                // Debug logging
                if (!string.IsNullOrEmpty(status.Token) || !string.IsNullOrEmpty(status.CardId))
                {
                    logger.LogInformation("Poll response - Token: {Token}, CardId: {CardId}", 
                        string.IsNullOrEmpty(status.Token) ? "empty" : status.Token.Substring(0, 10) + "...",
                        string.IsNullOrEmpty(status.CardId) ? "empty" : status.CardId);
                }
                
                if (!string.IsNullOrEmpty(status.Token) && !string.IsNullOrEmpty(status.CardId))
                {
                    detectedCardId = status.Token;
                    Console.WriteLine($"\n✓ Card detected! Card ID: {status.CardId}");
                    break;
                }
                else
                {
                    Console.Write(".");
                }
            }
            catch (UnifiAccessException ex) when (ex.ErrorCode == "CODE_CREDS_NFC_READ_POLL_TOKEN_EMPTY")
            {
                // This is normal while waiting for a card tap - the reader hasn't detected a card yet
                Console.Write(".");
                logger.LogDebug("Polling: No card detected yet");
            }
            catch (UnifiAccessException ex) when (ex.ErrorCode == "CODE_CREDS_NFC_CARD_IS_PROVISION")
            {
                // Card is already enrolled to another user
                cardAlreadyEnrolled = true;
                Console.WriteLine($"\n⚠️  This card is already enrolled to another user.");
                
                // Still need to get the card token from the session
                try
                {
                    var status = await client.Credentials.GetNfcEnrollmentStatusAsync(session.SessionId);
                    if (!string.IsNullOrEmpty(status.Token))
                    {
                        detectedCardId = status.Token;
                        break;
                    }
                }
                catch { }
                
                Console.WriteLine("Unable to retrieve card token. Please try again.");
                await client.Credentials.CancelNfcEnrollmentSessionAsync(session.SessionId);
                return;
            }
            catch (UnifiAccessException ex) when (ex.ErrorCode == "CODE_CREDS_NFC_READ_SESSION_NOT_FOUND")
            {
                // Session expired or was cancelled
                Console.WriteLine($"\n✗ Enrollment session expired or was cancelled.");
                return;
            }
            catch (UnifiAccessException ex) when (ex.ErrorCode == "CODE_CREDS_NFC_CARD_PROVISION_FAILED")
            {
                // Card was detected but not held long enough
                Console.WriteLine($"\n⚠️  Card detected but wasn't held long enough!");
                Console.WriteLine("   ➤ Hold the card firmly against the reader for at least 5 seconds");
                Console.WriteLine("   ➤ Wait for the beep/light confirmation");
                Console.WriteLine("   ➤ Press 'R' to retry with a new session, or wait to try again");
                Console.Write("\n   Continuing to poll");
                // Continue polling - don't break the loop
            }
            catch (UnifiAccessException ex) when (ex.ErrorCode == "CODE_CREDS_NFC_CARD_INVALID")
            {
                // Unsupported card type
                Console.WriteLine($"\n✗ Card type not supported. Please use a UniFi Access card.");
                await client.Credentials.CancelNfcEnrollmentSessionAsync(session.SessionId);
                return;
            }
            catch (System.Text.Json.JsonException jex)
            {
                // JSON deserialization error
                logger.LogError(jex, "JSON deserialization failed during polling");
                Console.WriteLine($"\n❌ JSON Error: {jex.Message}");
                Console.Write("   Continuing to poll...");
            }
            catch (UnifiAccessException ex)
            {
                // Log unexpected errors but continue polling unless critical
                logger.LogWarning("Polling error: {ErrorCode} - {Message}", ex.ErrorCode, ex.Message);
                Console.Write("!");  // Show something went wrong but keep trying
            }
        }
        
        if (string.IsNullOrEmpty(detectedCardId))
        {
            Console.WriteLine("\n✗ Timeout: No card was detected.");
            await client.Credentials.CancelNfcEnrollmentSessionAsync(session.SessionId);
            return;
        }

        // Determine if we need to force assignment
        bool forceAssign = false;
        if (cardAlreadyEnrolled)
        {
            Console.WriteLine("\n⚠️  This card is already assigned to another user.");
            Console.Write("Do you want to FORCE reassignment to the new user? (Y/N): ");
            var response = Console.ReadLine()?.Trim()?.ToUpperInvariant();
            
            if (response != "Y")
            {
                Console.WriteLine("✗ Assignment cancelled.");
                await client.Credentials.CancelNfcEnrollmentSessionAsync(session.SessionId);
                return;
            }
            
            forceAssign = true;
            Console.WriteLine("✓ Will force reassignment of the card.");
        }
        
        // Assign card to user
        logger.LogInformation("Assigning NFC card to user...");
        var assignRequest = new AssignNfcCardRequest
        {
            Token = detectedCardId,
            ForceAdd = forceAssign
        };
        
        try
        {
            await client.Users.AssignNfcCardToUserAsync(userId, assignRequest);
            Console.WriteLine($"\n✓ NFC card successfully {(forceAssign ? "reassigned" : "assigned")} to user!");
            Console.WriteLine($"  Card Token: {detectedCardId}");
        }
        catch (UnifiAccessException ex) when (ex.ErrorCode == "CODE_CREDS_NFC_CARD_IS_PROVISION" && !forceAssign)
        {
            // Card is already enrolled and we didn't force
            Console.WriteLine($"\n⚠️  Card is already enrolled to another user.");
            Console.Write("Do you want to FORCE reassignment? (Y/N): ");
            
            if (Console.ReadLine()?.Trim()?.ToUpperInvariant() == "Y")
            {
                assignRequest.ForceAdd = true;
                await client.Users.AssignNfcCardToUserAsync(userId, assignRequest);
                Console.WriteLine($"\n✓ NFC card successfully reassigned to user!");
                Console.WriteLine($"  Card Token: {detectedCardId}");
            }
            else
            {
                Console.WriteLine("✗ Assignment cancelled.");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to register NFC card");
        Console.WriteLine($"Error registering NFC card: {ex.Message}");
    }
}

async Task ImportNfcCards(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n--- Import NFC Cards from CSV ---");
    Console.WriteLine("CSV Format: nfc_id,alias");
    Console.WriteLine("Example:");
    Console.WriteLine("1234567890,John's Card");
    Console.WriteLine("0987654321,Jane's Card\n");
    
    Console.Write("Enter CSV file path (or press Enter to create sample): ");
    var filePath = Console.ReadLine()?.Trim();
    
    byte[] csvContent;
    
    if (string.IsNullOrWhiteSpace(filePath))
    {
        // Create sample CSV
        Console.WriteLine("\nEnter NFC cards (one per line, format: nfc_id,alias)");
        Console.WriteLine("Press Enter twice when done:\n");
        
        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("nfc_id,alias");
        
        string? line;
        while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
        {
            csvBuilder.AppendLine(line);
        }
        
        csvContent = Encoding.UTF8.GetBytes(csvBuilder.ToString());
    }
    else
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: File not found: {filePath}");
            return;
        }
        
        csvContent = await File.ReadAllBytesAsync(filePath);
    }

    try
    {
        var request = new ImportNfcCardsRequest
        {
            FileContent = csvContent,
            FileName = "nfc_cards_import.csv"
        };

        logger.LogInformation("Importing NFC cards...");
        var results = await client.Credentials.ImportNfcCardsAsync(request);
        
        var importedCards = results.ToList();
        Console.WriteLine($"\n✓ Import completed!");
        Console.WriteLine($"  Total cards processed: {importedCards.Count}");
        
        foreach (var card in importedCards)
        {
            if (!string.IsNullOrEmpty(card.Token))
            {
                Console.WriteLine($"  ✓ {card.NfcId} ({card.Alias ?? "No alias"}) - Token: {card.Token}");
            }
            else
            {
                Console.WriteLine($"  ✗ {card.NfcId} - Failed to import");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to import NFC cards");
        Console.WriteLine($"Error importing NFC cards: {ex.Message}");
    }
}

async Task ListNfcCards(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n--- List All NFC Cards ---");
    
    try
    {
        logger.LogInformation("Fetching NFC cards...");
        var cardsResponse = await client.Credentials.GetNfcCardsAsync();
        var cards = cardsResponse.Items;

        if (!cards.Any())
        {
            Console.WriteLine("No NFC cards found.");
            return;
        }

        Console.WriteLine($"\nFound {cards.Count} NFC cards (Page {cardsResponse.Page} of {cardsResponse.TotalPages}):\n");
        
        foreach (var card in cards)
        {
            Console.WriteLine($"• Card: {card.DisplayId}");
            Console.WriteLine($"  Token: {card.Token}");
            Console.WriteLine($"  Status: {card.Status}");
            Console.WriteLine($"  Alias: {card.Alias ?? "N/A"}");
            if (card.User != null)
            {
                Console.WriteLine($"  Assigned to: {card.User.FirstName} {card.User.LastName} ({card.User.Id})");
            }
            else
            {
                Console.WriteLine($"  Assigned to: Not assigned");
            }
            Console.WriteLine();
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to list NFC cards");
        Console.WriteLine($"Error listing NFC cards: {ex.Message}");
    }
}

async Task AssignExistingNfcCard(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n--- Assign Existing NFC Card to User ---");
    
    // List available cards
    logger.LogInformation("Fetching available NFC cards...");
    var cardsResponse = await client.Credentials.GetNfcCardsAsync();
    var cards = cardsResponse.Items;
    var availableCards = cards.Where(c => c.User == null).ToList();

    if (!availableCards.Any())
    {
        Console.WriteLine("No unassigned NFC cards available.");
        Console.WriteLine("\nWould you like to reassign an already assigned card? (Y/N): ");
        if (Console.ReadLine()?.Trim()?.ToUpperInvariant() == "Y")
        {
            availableCards = cards.ToList();
        }
        else
        {
            return;
        }
    }
    
    Console.WriteLine("\nAvailable NFC Cards:");
    for (int i = 0; i < availableCards.Count; i++)
    {
        var card = availableCards[i];
        var assignedInfo = card.User != null ? $" (currently: {card.User.FirstName} {card.User.LastName})" : "";
        Console.WriteLine($"{i + 1}. {card.DisplayId} - {card.Alias ?? "No alias"}{assignedInfo}");
    }
    
    Console.Write("\nSelect card number: ");
    if (!int.TryParse(Console.ReadLine(), out int cardIndex) || cardIndex < 1 || cardIndex > availableCards.Count)
    {
        Console.WriteLine("Invalid card selection.");
        return;
    }
    
    var selectedCard = availableCards[cardIndex - 1];
    
    // Select user
    Console.Write("\nEnter User ID (or press Enter to list users): ");
    var userId = Console.ReadLine()?.Trim();
    
    if (string.IsNullOrWhiteSpace(userId))
    {
        await ListUsers(client, logger);
        Console.Write("\nEnter User ID: ");
        userId = Console.ReadLine()?.Trim();
    }
    
    if (string.IsNullOrWhiteSpace(userId))
    {
        Console.WriteLine("Error: User ID is required.");
        return;
    }

    try
    {
        var forceAdd = selectedCard.User != null;
        
        logger.LogInformation("Assigning NFC card to user...");
        var request = new AssignNfcCardRequest
        {
            Token = selectedCard.Token,
            ForceAdd = forceAdd
        };
        
        await client.Users.AssignNfcCardToUserAsync(userId, request);
        
        Console.WriteLine($"\n✓ NFC card successfully assigned to user!");
        Console.WriteLine($"  Card: {selectedCard.DisplayId}");
        Console.WriteLine($"  Token: {selectedCard.Token}");
        if (forceAdd)
        {
            Console.WriteLine($"  Note: Card was reassigned from another user");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to assign NFC card");
        Console.WriteLine($"Error assigning NFC card: {ex.Message}");
    }
}

async Task RemoveNfcCardFromUser(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n--- Remove NFC Card from User ---");
    
    Console.Write("Enter User ID (or press Enter to list users): ");
    var userId = Console.ReadLine()?.Trim();
    
    if (string.IsNullOrWhiteSpace(userId))
    {
        await ListUsers(client, logger);
        Console.Write("\nEnter User ID: ");
        userId = Console.ReadLine()?.Trim();
    }
    
    if (string.IsNullOrWhiteSpace(userId))
    {
        Console.WriteLine("Error: User ID is required.");
        return;
    }

    try
    {
        // Get user to see their cards
        logger.LogInformation("Fetching user details...");
        var user = await client.Users.GetUserAsync(userId);
        
        if (user.NfcCards == null || !user.NfcCards.Any())
        {
            Console.WriteLine("This user has no NFC cards assigned.");
            return;
        }
        
        Console.WriteLine($"\nUser: {user.FirstName} {user.LastName}");
        Console.WriteLine("Assigned NFC Cards:");
        for (int i = 0; i < user.NfcCards.Count; i++)
        {
            var card = user.NfcCards[i];
            Console.WriteLine($"{i + 1}. {card.Id} - {card.Type ?? "Unknown type"}");
        }
        
        Console.Write("\nSelect card number to remove: ");
        if (!int.TryParse(Console.ReadLine(), out int cardIndex) || cardIndex < 1 || cardIndex > user.NfcCards.Count)
        {
            Console.WriteLine("Invalid card selection.");
            return;
        }
        
        var cardToRemove = user.NfcCards[cardIndex - 1];
        
        logger.LogInformation("Removing NFC card from user...");
        await client.Users.UnassignNfcCardFromUserAsync(userId, cardToRemove.Id);
        
        Console.WriteLine($"\n✓ NFC card successfully removed from user!");
        Console.WriteLine($"  Card ID: {cardToRemove.Id}");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to remove NFC card");
        Console.WriteLine($"Error removing NFC card: {ex.Message}");
    }
}

async Task AssignPinCode(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n--- Assign PIN Code ---");
    
    Console.WriteLine("\nSelect user for PIN assignment:");
    Console.WriteLine("1. Enter User ID directly");
    Console.WriteLine("2. Search for user by name");
    Console.WriteLine("3. List all users");
    Console.Write("\nSelect option (1-3): ");
    
    var option = Console.ReadLine()?.Trim();
    string? userId = null;
    
    switch (option)
    {
        case "1":
            Console.Write("Enter User ID: ");
            userId = Console.ReadLine()?.Trim();
            break;
            
        case "2":
            Console.Write("Enter search keyword (name/email): ");
            var keyword = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                logger.LogInformation("Searching for users...");
                var searchResults = await client.Users.SearchUsersAsync(keyword);
                
                if (!searchResults.Any())
                {
                    Console.WriteLine("No users found matching the search criteria.");
                    return;
                }
                
                Console.WriteLine($"\nFound {searchResults.Count()} user(s):\n");
                var userList = searchResults.ToList();
                for (int i = 0; i < userList.Count; i++)
                {
                    var user = userList[i];
                    Console.WriteLine($"{i + 1}. {user.FirstName} {user.LastName}");
                    Console.WriteLine($"   ID: {user.Id}");
                    Console.WriteLine($"   Email: {user.UserEmail ?? "N/A"}");
                    Console.WriteLine($"   Employee #: {user.EmployeeNumber ?? "N/A"}");
                }
                
                Console.Write("\nSelect user number: ");
                if (int.TryParse(Console.ReadLine(), out int userIndex) && userIndex > 0 && userIndex <= userList.Count)
                {
                    userId = userList[userIndex - 1].Id;
                }
            }
            break;
            
        case "3":
            await ListUsers(client, logger);
            Console.Write("\nEnter User ID from the list: ");
            userId = Console.ReadLine()?.Trim();
            break;
            
        default:
            Console.WriteLine("Invalid option.");
            return;
    }
    
    if (string.IsNullOrWhiteSpace(userId))
    {
        Console.WriteLine("Error: User ID is required.");
        return;
    }

    try
    {
        Console.WriteLine("\n1. Generate random PIN");
        Console.WriteLine("2. Enter custom PIN");
        Console.Write("\nSelect option: ");
        
        var pinOption = Console.ReadLine()?.Trim();
        string pinCode;
        
        if (pinOption == "1")
        {
            logger.LogInformation("Generating random PIN code...");
            pinCode = await client.Credentials.GeneratePinCodeAsync();
            Console.WriteLine($"\n✓ Generated PIN: {pinCode}");
        }
        else if (pinOption == "2")
        {
            Console.Write("Enter PIN code (4-8 digits): ");
            pinCode = Console.ReadLine()?.Trim() ?? "";
            
            if (pinCode.Length < 4 || pinCode.Length > 8 || !pinCode.All(char.IsDigit))
            {
                Console.WriteLine("Error: PIN must be 4-8 digits.");
                return;
            }
        }
        else
        {
            Console.WriteLine("Invalid option.");
            return;
        }

        logger.LogInformation("Assigning PIN code to user...");
        var request = new AssignPinCodeRequest
        {
            PinCode = pinCode
        };
        
        await client.Users.AssignPinCodeToUserAsync(userId, request);
        
        Console.WriteLine($"\n✓ PIN code successfully assigned to user!");
        Console.WriteLine($"  PIN: {pinCode}");
        Console.WriteLine("\n⚠️  Important: Please provide this PIN to the user securely.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to assign PIN code");
        Console.WriteLine($"Error assigning PIN code: {ex.Message}");
    }
}

async Task CompleteOnboardingWorkflow(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n╔════════════════════════════════════════════╗");
    Console.WriteLine("║     EMPLOYEE ONBOARDING WORKFLOW           ║");
    Console.WriteLine("╚════════════════════════════════════════════╝");
    Console.WriteLine("\nThis workflow will:");
    Console.WriteLine("  1. Create a new user");
    Console.WriteLine("  2. Assign them to a user group (with policies)");
    Console.WriteLine("  3. Register an NFC card for access");
    Console.WriteLine("\nPress Enter to begin or Ctrl+C to cancel...");
    Console.ReadLine();

    try
    {
        // ═══════════════════════════════════════════════════════
        // STEP 1: CREATE NEW USER
        // ═══════════════════════════════════════════════════════
        Console.WriteLine("\n┌─────────────────────────────────────┐");
        Console.WriteLine("│  STEP 1: Create New User            │");
        Console.WriteLine("└─────────────────────────────────────┘");
        
        Console.Write("\nFirst Name: ");
        var firstName = Console.ReadLine()?.Trim();
        Console.Write("Last Name: ");
        var lastName = Console.ReadLine()?.Trim();
        Console.Write("Email (optional): ");
        var email = Console.ReadLine()?.Trim();
        Console.Write("Employee ID (optional): ");
        var employeeId = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            Console.WriteLine("\n❌ Error: First and Last name are required.");
            return;
        }

        // Try to create user or use existing one
        UserResponse user;
        bool userAlreadyExists = false;
        
        try
        {
            logger.LogInformation("Creating user {FirstName} {LastName}...", firstName, lastName);
            
            var userRequest = new CreateUserRequest
            {
                FirstName = firstName,
                LastName = lastName,
                UserEmail = string.IsNullOrWhiteSpace(email) ? null : email,
                EmployeeNumber = string.IsNullOrWhiteSpace(employeeId) ? null : employeeId
            };

            user = await client.Users.CreateUserAsync(userRequest);
            Console.WriteLine($"\n✅ User created successfully!");
            Console.WriteLine($"   User ID: {user.Id}");
            Console.WriteLine($"   Name: {user.FirstName} {user.LastName}");
        }
        catch (UnifiValidationException ex) when (ex.ErrorCode == "CODE_USER_EMPLOYEE_NUMBER_EXIST")
        {
            Console.WriteLine($"\n⚠️  Employee ID {employeeId} already exists.");
            Console.Write("Do you want to continue with the existing user? (Y/N): ");
            
            if (Console.ReadLine()?.Trim()?.ToUpperInvariant() != "Y")
            {
                Console.WriteLine("❌ Onboarding cancelled.");
                return;
            }
            
            // Find the existing user by employee ID
            logger.LogInformation("Searching for existing user with employee ID {EmployeeId}...", employeeId);
            var allUsers = await client.Users.GetUsersAsync();
            var existingUser = allUsers.FirstOrDefault(u => u.EmployeeNumber == employeeId);
            
            if (existingUser == null)
            {
                Console.WriteLine("❌ Could not find user with that employee ID.");
                return;
            }
            
            user = existingUser;
            
            userAlreadyExists = true;
            Console.WriteLine($"\n✅ Found existing user:");
            Console.WriteLine($"   User ID: {user.Id}");
            Console.WriteLine($"   Name: {user.FirstName} {user.LastName}");
            Console.WriteLine($"   Email: {user.UserEmail ?? "Not provided"}");
        }

        // ═══════════════════════════════════════════════════════
        // STEP 2: ASSIGN TO USER GROUP
        // ═══════════════════════════════════════════════════════
        Console.WriteLine("\n┌─────────────────────────────────────┐");
        Console.WriteLine("│  STEP 2: Assign to User Group       │");
        Console.WriteLine("└─────────────────────────────────────┘");
        
        if (userAlreadyExists)
        {
            Console.WriteLine("\n⚠️  User already exists. They may already be assigned to groups.");
            Console.Write("Do you want to add them to another group? (Y/N): ");
            
            if (Console.ReadLine()?.Trim()?.ToUpperInvariant() != "Y")
            {
                Console.WriteLine("➤ Skipping group assignment.");
                goto SkipGroupAssignment;
            }
        }
        
        logger.LogInformation("Fetching user groups...");
        var groupsResponse = await client.UserGroups.GetUserGroupsAsync();
        var groupList = groupsResponse.Items;
        
        if (!groupList.Any())
        {
            Console.WriteLine("\n⚠️  No user groups available. Skipping group assignment.");
        }
        else
        {
            Console.WriteLine("\nAvailable User Groups (with access policies):");
            Console.WriteLine("──────────────────────────────────────────────");
            
            for (int i = 0; i < groupList.Count; i++)
            {
                var group = groupList[i];
                Console.WriteLine($"\n[{i + 1}] {group.Name}");
                if (!string.IsNullOrWhiteSpace(group.Description))
                {
                    Console.WriteLine($"    Description: {group.Description}");
                }
            }
            
            Console.Write("\n➤ Select group number: ");
            if (int.TryParse(Console.ReadLine(), out int groupIndex) && groupIndex > 0 && groupIndex <= groupList.Count)
            {
                var selectedGroup = groupList[groupIndex - 1];
                
                logger.LogInformation("Adding user to group {GroupName}...", selectedGroup.Name);
                
                var assignRequest = new AssignUsersToGroupRequest
                {
                    UserIds = new List<string> { user.Id }
                };
                
                await client.UserGroups.AssignUsersToGroupAsync(selectedGroup.Id, assignRequest);
                Console.WriteLine($"\n✅ User added to group: {selectedGroup.Name}");
                Console.WriteLine("   (Access policies from this group are now active)");
            }
            else
            {
                Console.WriteLine("\n⚠️  Invalid selection. Skipping group assignment.");
            }
        }

        SkipGroupAssignment:
        
        // ═══════════════════════════════════════════════════════
        // STEP 3: REGISTER NFC CARD
        // ═══════════════════════════════════════════════════════
        Console.WriteLine("\n┌─────────────────────────────────────┐");
        Console.WriteLine("│  STEP 3: Register NFC Card          │");
        Console.WriteLine("└─────────────────────────────────────┘");
        
        Console.WriteLine("\nWould you like to register an NFC card for this user? (Y/N): ");
        var registerCard = Console.ReadLine()?.Trim()?.ToUpperInvariant();
        
        string? nfcOption = registerCard == "Y" ? "1" : "skip";
        
        if (nfcOption == "1")
        {
            // Use our existing NFC enrollment function but with the specific user ID
            await RegisterNfcCardForUser(client, logger, user.Id, firstName, lastName);
        }
        else
        {
            Console.WriteLine("➤ Skipping NFC card registration.");
        }

        // ═══════════════════════════════════════════════════════
        // ONBOARDING COMPLETE
        // ═══════════════════════════════════════════════════════
        Console.WriteLine("\n╔════════════════════════════════════════════╗");
        Console.WriteLine("║     ONBOARDING COMPLETE! 🎉                ║");
        Console.WriteLine("╚════════════════════════════════════════════╝");
        
        Console.WriteLine($"\n📋 Summary:");
        Console.WriteLine($"   Name: {firstName} {lastName}");
        Console.WriteLine($"   User ID: {user.Id}");
        Console.WriteLine($"   Email: {email ?? "Not provided"}");
        Console.WriteLine($"   Employee ID: {employeeId ?? "Not provided"}");
        Console.WriteLine($"\n✅ User is ready for access!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Onboarding workflow failed");
        Console.WriteLine($"\nError: {ex.Message}");
    }
}

async Task BulkImportUsersAndCards(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n=== Bulk Import Users and Cards ===");
    Console.WriteLine("This will import multiple users and their NFC cards from CSV data.");
    Console.WriteLine("\nExample CSV format:");
    Console.WriteLine("FirstName,LastName,Email,EmployeeID,NFCCardID,GroupName");
    Console.WriteLine("John,Doe,john@company.com,EMP001,1234567890,Engineering");
    Console.WriteLine("Jane,Smith,jane@company.com,EMP002,0987654321,Sales\n");

    Console.Write("Enter CSV file path (or press Enter to use sample data): ");
    var filePath = Console.ReadLine()?.Trim();

    List<(string firstName, string lastName, string email, string employeeId, string nfcId, string groupName)> importData;

    if (string.IsNullOrWhiteSpace(filePath))
    {
        // Use sample data
        importData = new List<(string, string, string, string, string, string)>
        {
            ("John", "Doe", "john@company.com", "EMP001", "1234567890", "Engineering"),
            ("Jane", "Smith", "jane@company.com", "EMP002", "0987654321", "Sales"),
            ("Bob", "Johnson", "bob@company.com", "EMP003", "1122334455", "Engineering")
        };
        
        Console.WriteLine("\nUsing sample data:");
        foreach (var item in importData)
        {
            Console.WriteLine($"  {item.firstName} {item.lastName} - {item.nfcId}");
        }
    }
    else
    {
        // Read from file
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: File not found: {filePath}");
            return;
        }

        importData = new List<(string, string, string, string, string, string)>();
        var lines = await File.ReadAllLinesAsync(filePath);
        
        // Skip header if present
        var startIndex = lines[0].Contains("FirstName") ? 1 : 0;
        
        for (int i = startIndex; i < lines.Length; i++)
        {
            var parts = lines[i].Split(',');
            if (parts.Length >= 6)
            {
                importData.Add((parts[0], parts[1], parts[2], parts[3], parts[4], parts[5]));
            }
        }
    }

    Console.WriteLine($"\nProcessing {importData.Count} records...\n");

    try
    {
        // Step 1: Create/Get User Groups
        var groupMap = new Dictionary<string, string>(); // groupName -> groupId
        var existingGroupsResponse = await client.UserGroups.GetUserGroupsAsync();
        var existingGroups = existingGroupsResponse.Items;

        foreach (var groupName in importData.Select(d => d.groupName).Distinct())
        {
            var existingGroup = existingGroups.FirstOrDefault(g => g.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase));
            
            if (existingGroup != null)
            {
                groupMap[groupName] = existingGroup.Id;
            }
            else
            {
                // Create new group
                logger.LogInformation("Creating group: {GroupName}", groupName);
                var groupRequest = new UserGroupRequest
                {
                    Name = groupName,
                    Description = $"Imported group for {groupName}"
                };
                
                var newGroup = await client.UserGroups.CreateUserGroupAsync(groupRequest);
                groupMap[groupName] = newGroup.Id;
                Console.WriteLine($"✓ Created group: {groupName}");
            }
        }

        // Step 2: Import all NFC cards
        var nfcCsvBuilder = new StringBuilder();
        nfcCsvBuilder.AppendLine("nfc_id,alias");
        
        foreach (var record in importData)
        {
            nfcCsvBuilder.AppendLine($"{record.nfcId},{record.firstName}'s Card");
        }
        
        var nfcImportRequest = new ImportNfcCardsRequest
        {
            FileContent = Encoding.UTF8.GetBytes(nfcCsvBuilder.ToString()),
            FileName = "bulk_nfc_import.csv"
        };
        
        logger.LogInformation("Importing NFC cards...");
        var importedCards = (await client.Credentials.ImportNfcCardsAsync(nfcImportRequest)).ToList();
        
        var cardTokenMap = importedCards.ToDictionary(c => c.NfcId, c => c.Token);
        Console.WriteLine($"✓ Imported {importedCards.Count} NFC cards");

        // Step 3: Create users and assign cards/groups
        var successCount = 0;
        var failedUsers = new List<string>();
        
        foreach (var record in importData)
        {
            try
            {
                // Create user
                var userRequest = new CreateUserRequest
                {
                    FirstName = record.firstName,
                    LastName = record.lastName,
                    UserEmail = record.email,
                    EmployeeNumber = record.employeeId
                };
                
                var user = await client.Users.CreateUserAsync(userRequest);
                
                // Assign NFC card if imported successfully
                if (cardTokenMap.TryGetValue(record.nfcId, out var cardToken) && !string.IsNullOrEmpty(cardToken))
                {
                    var assignCardRequest = new AssignNfcCardRequest
                    {
                        Token = cardToken,
                        ForceAdd = true // Force in case of reassignment
                    };
                    
                    await client.Users.AssignNfcCardToUserAsync(user.Id, assignCardRequest);
                }
                
                // Add to group
                if (groupMap.TryGetValue(record.groupName, out var groupId))
                {
                    var assignGroupRequest = new AssignUsersToGroupRequest
                    {
                        UserIds = new List<string> { user.Id }
                    };
                    
                    await client.UserGroups.AssignUsersToGroupAsync(groupId, assignGroupRequest);
                }
                
                // Generate PIN
                var pinCode = await client.Credentials.GeneratePinCodeAsync();
                var pinRequest = new AssignPinCodeRequest
                {
                    PinCode = pinCode
                };
                
                await client.Users.AssignPinCodeToUserAsync(user.Id, pinRequest);
                
                Console.WriteLine($"✓ {record.firstName} {record.lastName} - User: {user.Id}, PIN: {pinCode}");
                successCount++;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process user {FirstName} {LastName}", record.firstName, record.lastName);
                failedUsers.Add($"{record.firstName} {record.lastName}: {ex.Message}");
            }
        }

        // Summary
        Console.WriteLine("\n=== Import Summary ===");
        Console.WriteLine($"Total Records: {importData.Count}");
        Console.WriteLine($"Successful: {successCount}");
        Console.WriteLine($"Failed: {failedUsers.Count}");
        
        if (failedUsers.Any())
        {
            Console.WriteLine("\nFailed imports:");
            foreach (var failure in failedUsers)
            {
                Console.WriteLine($"  ✗ {failure}");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Bulk import failed");
        Console.WriteLine($"\nError: {ex.Message}");
    }
}

async Task ListDevices(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n--- List Devices ---");

    try
    {
        logger.LogInformation("Fetching devices...");
        var devices = await client.Devices.GetDevicesAsync();

        if (!devices.Any())
        {
            Console.WriteLine("No devices found.");
            return;
        }

        Console.WriteLine($"\nFound {devices.Count()} devices:\n");

        foreach (var device in devices)
        {
            Console.WriteLine($"• {device.Name}");
            Console.WriteLine($"  ID: {device.Id}");
            Console.WriteLine($"  Type: {device.Type}");
            Console.WriteLine($"  Alias: {device.Alias ?? "N/A"}");
            Console.WriteLine();
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to list devices");
        Console.WriteLine($"Error listing devices: {ex.Message}");
    }
}

async Task ExportLastActivityReport(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n--- Export Last Activity Report (CSV) ---");

    try
    {
        // Fetch all users first
        logger.LogInformation("Fetching all users...");
        var allUsers = await client.Users.GetUsersAsync();
        var userList = allUsers.ToList();

        Console.WriteLine($"Found {userList.Count} users. Fetching all door opening logs...");

        // Fetch door opening logs (without date restriction for now to get all activity)
        var logRequest = new SystemLogRequest
        {
            Topic = SystemLogTopic.DoorOpenings
        };

        // Dictionary to track last activity per user
        var userLastActivity = new Dictionary<string, DateTime>();

        // Fetch logs in pages
        int pageNum = 1;
        const int pageSize = 100;
        SystemLogQueryResponse logResponse;
        var totalLogs = 0;

        do
        {
            logResponse = await client.SystemLogs.GetSystemLogsAsync(logRequest, pageNum, pageSize);
            totalLogs += logResponse.Hits.Count;

            Console.Write($"Processing page {pageNum} ({logResponse.Hits.Count} logs)...");

            foreach (var entry in logResponse.Hits)
            {
                // Only process user-type actors with successful access
                if (entry.Source.Actor.Type == "user" &&
                    !string.IsNullOrEmpty(entry.Source.Actor.Id) &&
                    entry.Source.Event.Result == "ACCESS")
                {
                    var userId = entry.Source.Actor.Id;

                    // Parse timestamp
                    if (DateTime.TryParse(entry.Timestamp, out var activityTime))
                    {
                        // Keep only the most recent activity for each user
                        if (!userLastActivity.ContainsKey(userId) || activityTime > userLastActivity[userId])
                        {
                            userLastActivity[userId] = activityTime;
                        }
                    }
                }
            }

            Console.WriteLine($" Found {userLastActivity.Count} unique active users so far.");
            pageNum++;

        } while (pageNum <= (logResponse.Total + pageSize - 1) / pageSize && logResponse.Hits.Count > 0);

        Console.WriteLine($"\nProcessed {totalLogs} total log entries.");
        Console.WriteLine($"Generating CSV report for {userList.Count} users...");

        // Create CSV content
        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("Name,Email,Employee ID,Last Activity,Days Since Activity,Status");

        // Prepare user activity data
        var userActivityData = new List<(UserResponse user, DateTime? lastActivity, int daysSince)>();

        foreach (var user in userList)
        {
            DateTime? lastActivity = null;
            int daysSince = -1;

            if (userLastActivity.TryGetValue(user.Id, out var activity))
            {
                lastActivity = activity;
                daysSince = (int)(DateTime.UtcNow - activity).TotalDays;
            }

            userActivityData.Add((user, lastActivity, daysSince));
        }

        // Sort by last activity (oldest first), users with no activity go to the end
        var sortedUsers = userActivityData
            .OrderBy(x => x.lastActivity.HasValue ? 0 : 1) // Users with activity first
            .ThenBy(x => x.lastActivity) // Then by oldest activity first
            .ToList();

        // Generate CSV rows
        foreach (var (user, lastActivity, daysSince) in sortedUsers)
        {
            var name = $"{user.FirstName} {user.LastName}";
            var email = user.UserEmail ?? "N/A";
            var employeeId = user.EmployeeNumber ?? "N/A";
            var lastActivityStr = lastActivity?.ToString("yyyy-MM-dd HH:mm:ss UTC") ?? "No activity";
            var daysSinceStr = daysSince >= 0 ? daysSince.ToString() : "N/A";
            var status = user.Status ?? "Unknown";

            // Escape CSV values that contain commas
            name = EscapeCsvValue(name);
            email = EscapeCsvValue(email);
            employeeId = EscapeCsvValue(employeeId);
            lastActivityStr = EscapeCsvValue(lastActivityStr);
            status = EscapeCsvValue(status);

            csvBuilder.AppendLine($"{name},{email},{employeeId},{lastActivityStr},{daysSinceStr},{status}");
        }

        // Save CSV to file
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var fileName = $"LastActivityReport_{timestamp}.csv";
        var filePath = Path.Combine(Environment.CurrentDirectory, fileName);

        await File.WriteAllTextAsync(filePath, csvBuilder.ToString());

        // Display summary
        var usersWithActivity = userActivityData.Count(x => x.lastActivity.HasValue);
        var usersWithoutActivity = userList.Count - usersWithActivity;

        Console.WriteLine($"\n✅ Last Activity Report exported successfully!");
        Console.WriteLine($"   File: {filePath}");
        Console.WriteLine($"   Total Users: {userList.Count}");
        Console.WriteLine($"   Users with Activity: {usersWithActivity}");
        Console.WriteLine($"   Users without Activity: {usersWithoutActivity}");
        Console.WriteLine($"   Report covers: All available activity logs");

        if (usersWithActivity > 0)
        {
            var oldestActivity = userActivityData
                .Where(x => x.lastActivity.HasValue)
                .Min(x => x.lastActivity!.Value);
            var newestActivity = userActivityData
                .Where(x => x.lastActivity.HasValue)
                .Max(x => x.lastActivity!.Value);

            Console.WriteLine($"   Activity Range: {oldestActivity:yyyy-MM-dd HH:mm} to {newestActivity:yyyy-MM-dd HH:mm}");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to export last activity report");
        Console.WriteLine($"Error exporting report: {ex.Message}");
    }
}

static string EscapeCsvValue(string value)
{
    if (string.IsNullOrEmpty(value))
    {
        return value;
    }

    // If the value contains comma, quote, or newline, wrap it in quotes
    if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
    {
        // Escape quotes by doubling them
        value = value.Replace("\"", "\"\"");
        return $"\"{value}\"";
    }

    return value;
}