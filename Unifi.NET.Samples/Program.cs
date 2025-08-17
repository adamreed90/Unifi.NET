using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Unifi.NET.Access;
using Unifi.NET.Access.Configuration;
using Unifi.NET.Access.Models.Credentials;
using Unifi.NET.Access.Models.Users;
using Unifi.NET.Access.Models.UserGroups;
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
        Console.WriteLine("17. Exit");
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
        var groups = await client.UserGroups.GetUserGroupsAsync();
        
        if (!groups.Any())
        {
            Console.WriteLine("No user groups found.");
            return;
        }

        Console.WriteLine($"\nFound {groups.Count()} user groups:\n");
        
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
        var users = await client.UserGroups.GetUsersInGroupAsync(groupId);
        
        if (!users.Any())
        {
            Console.WriteLine("No users found in this group.");
            return;
        }

        Console.WriteLine($"\nFound {users.Count()} users in group:\n");
        
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

async Task RegisterNfcCard(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n--- Register NFC Card via Enrollment ---");
    
    // First, list users to select from
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

    // Get available devices
    logger.LogInformation("Fetching available devices...");
    var devices = await client.Devices.GetDevicesAsync();
    var nfcDevices = devices.Where(d => d.Type == "UAH" || d.Type == "UAHP").ToList();
    
    if (!nfcDevices.Any())
    {
        Console.WriteLine("Error: No NFC-capable devices found.");
        return;
    }

    Console.WriteLine("\nAvailable NFC Devices:");
    for (int i = 0; i < nfcDevices.Count; i++)
    {
        var device = nfcDevices[i];
        Console.WriteLine($"{i + 1}. {device.Name} ({device.Type})");
    }
    
    Console.Write("\nSelect device number: ");
    if (!int.TryParse(Console.ReadLine(), out int deviceIndex) || deviceIndex < 1 || deviceIndex > nfcDevices.Count)
    {
        Console.WriteLine("Invalid device selection.");
        return;
    }
    
    var selectedDevice = nfcDevices[deviceIndex - 1];

    try
    {
        // Create enrollment session
        logger.LogInformation("Creating NFC enrollment session...");
        var sessionRequest = new CreateNfcEnrollmentSessionRequest
        {
            DeviceId = selectedDevice.Id
        };
        
        var session = await client.Credentials.CreateNfcEnrollmentSessionAsync(sessionRequest);
        Console.WriteLine($"\n✓ Enrollment session created!");
        Console.WriteLine($"  Session ID: {session.SessionId}");
        Console.WriteLine($"\n⏳ Please tap your NFC card on device: {selectedDevice.Name}");
        Console.WriteLine("   Waiting for card detection (timeout: 60 seconds)...\n");

        // Poll for card detection
        var startTime = DateTime.UtcNow;
        var timeout = TimeSpan.FromSeconds(60);
        string? detectedCardId = null;
        
        while (DateTime.UtcNow - startTime < timeout)
        {
            await Task.Delay(2000); // Poll every 2 seconds
            
            var status = await client.Credentials.GetNfcEnrollmentStatusAsync(session.SessionId);
            
            if (!string.IsNullOrEmpty(status.Token) && !string.IsNullOrEmpty(status.CardId))
            {
                detectedCardId = status.Token;
                Console.WriteLine($"✓ Card detected! Card ID: {status.CardId}");
                break;
            }
            else if (status.Token == null && status.CardId == null)
            {
                Console.WriteLine("✗ Enrollment failed.");
                return;
            }
            else
            {
                Console.Write(".");
            }
        }
        
        if (string.IsNullOrEmpty(detectedCardId))
        {
            Console.WriteLine("\n✗ Timeout: No card was detected.");
            await client.Credentials.CancelNfcEnrollmentSessionAsync(session.SessionId);
            return;
        }

        // Assign card to user
        Console.Write("\nForce assign? (Y/N - use Y if card is already assigned): ");
        var forceAssign = Console.ReadLine()?.Trim()?.ToUpperInvariant() == "Y";
        
        logger.LogInformation("Assigning NFC card to user...");
        var assignRequest = new AssignNfcCardRequest
        {
            Token = detectedCardId,
            ForceAdd = forceAssign
        };
        
        await client.Users.AssignNfcCardToUserAsync(userId, assignRequest);
        
        Console.WriteLine($"\n✓ NFC card successfully assigned to user!");
        Console.WriteLine($"  Card Token: {detectedCardId}");
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
        var cards = await client.Credentials.GetNfcCardsAsync();
        
        if (!cards.Any())
        {
            Console.WriteLine("No NFC cards found.");
            return;
        }

        Console.WriteLine($"\nFound {cards.Count()} NFC cards:\n");
        
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
    var cards = await client.Credentials.GetNfcCardsAsync();
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
        Console.WriteLine("\n1. Generate random PIN");
        Console.WriteLine("2. Enter custom PIN");
        Console.Write("\nSelect option: ");
        
        var option = Console.ReadLine()?.Trim();
        string pinCode;
        
        if (option == "1")
        {
            logger.LogInformation("Generating random PIN code...");
            pinCode = await client.Credentials.GeneratePinCodeAsync();
            Console.WriteLine($"\n✓ Generated PIN: {pinCode}");
        }
        else if (option == "2")
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
    Console.WriteLine("\n=== Complete Employee Onboarding Workflow ===");
    Console.WriteLine("This workflow will:");
    Console.WriteLine("1. Create a new user");
    Console.WriteLine("2. Assign them to a user group");
    Console.WriteLine("3. Register an NFC card");
    Console.WriteLine("4. Generate a PIN code");
    Console.WriteLine("\nPress Enter to continue or Ctrl+C to cancel...");
    Console.ReadLine();

    try
    {
        // Step 1: Create User
        Console.WriteLine("\n[Step 1/4] Create New User");
        Console.WriteLine("------------------------");
        Console.Write("First Name: ");
        var firstName = Console.ReadLine()?.Trim();
        Console.Write("Last Name: ");
        var lastName = Console.ReadLine()?.Trim();
        Console.Write("Email: ");
        var email = Console.ReadLine()?.Trim();
        Console.Write("Employee ID: ");
        var employeeId = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            Console.WriteLine("Error: Name is required.");
            return;
        }

        var userRequest = new CreateUserRequest
        {
            FirstName = firstName,
            LastName = lastName,
            UserEmail = email,
            EmployeeNumber = employeeId
        };

        logger.LogInformation("Creating user...");
        var user = await client.Users.CreateUserAsync(userRequest);
        Console.WriteLine($"✓ User created: {user.Id}");

        // Step 2: Add to User Group
        Console.WriteLine("\n[Step 2/4] Add to User Group");
        Console.WriteLine("------------------------");
        
        var groups = await client.UserGroups.GetUserGroupsAsync();
        if (groups.Any())
        {
            Console.WriteLine("Available groups:");
            var groupList = groups.ToList();
            for (int i = 0; i < groupList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {groupList[i].Name}");
            }
            
            Console.Write("\nSelect group number (or 0 to skip): ");
            if (int.TryParse(Console.ReadLine(), out int groupIndex) && groupIndex > 0 && groupIndex <= groupList.Count)
            {
                var selectedGroup = groupList[groupIndex - 1];
                var assignRequest = new AssignUsersToGroupRequest
                {
                    UserIds = new List<string> { user.Id }
                };
                
                logger.LogInformation("Adding user to group...");
                await client.UserGroups.AssignUsersToGroupAsync(selectedGroup.Id, assignRequest);
                Console.WriteLine($"✓ User added to group: {selectedGroup.Name}");
            }
        }
        else
        {
            Console.WriteLine("No user groups available. Skipping...");
        }

        // Step 3: Register NFC Card
        Console.WriteLine("\n[Step 3/4] Register NFC Card");
        Console.WriteLine("------------------------");
        Console.WriteLine("Options:");
        Console.WriteLine("1. Enroll new card with reader");
        Console.WriteLine("2. Import existing card by ID");
        Console.WriteLine("3. Skip NFC card");
        Console.Write("\nSelect option: ");
        
        var nfcOption = Console.ReadLine()?.Trim();
        
        if (nfcOption == "1")
        {
            // Enroll with reader
            var devices = await client.Devices.GetDevicesAsync();
            var nfcDevices = devices.Where(d => d.Type == "UAH" || d.Type == "UAHP").ToList();
            
            if (nfcDevices.Any())
            {
                Console.WriteLine("\nNFC Devices:");
                for (int i = 0; i < nfcDevices.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {nfcDevices[i].Name}");
                }
                
                Console.Write("Select device: ");
                if (int.TryParse(Console.ReadLine(), out int deviceIndex) && deviceIndex > 0 && deviceIndex <= nfcDevices.Count)
                {
                    var device = nfcDevices[deviceIndex - 1];
                    var sessionRequest = new CreateNfcEnrollmentSessionRequest
                    {
                        DeviceId = device.Id
                    };
                    
                    var session = await client.Credentials.CreateNfcEnrollmentSessionAsync(sessionRequest);
                    Console.WriteLine($"\n⏳ Tap NFC card on {device.Name} within 60 seconds...");
                    
                    // Poll for enrollment
                    var startTime = DateTime.UtcNow;
                    while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(60))
                    {
                        await Task.Delay(2000);
                        var status = await client.Credentials.GetNfcEnrollmentStatusAsync(session.SessionId);
                        
                        if (!string.IsNullOrEmpty(status.Token))
                        {
                            var assignNfcRequest = new AssignNfcCardRequest
                            {
                                Token = status.Token
                            };
                            
                            await client.Users.AssignNfcCardToUserAsync(user.Id, assignNfcRequest);
                            Console.WriteLine($"✓ NFC card enrolled and assigned");
                            break;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No NFC devices available");
            }
        }
        else if (nfcOption == "2")
        {
            Console.Write("Enter NFC ID: ");
            var nfcId = Console.ReadLine()?.Trim();
            
            if (!string.IsNullOrWhiteSpace(nfcId))
            {
                // Import the card first
                var csvContent = Encoding.UTF8.GetBytes($"nfc_id,alias\n{nfcId},{firstName}'s Card");
                var importRequest = new ImportNfcCardsRequest
                {
                    FileContent = csvContent,
                    FileName = "import.csv"
                };
                
                var imported = await client.Credentials.ImportNfcCardsAsync(importRequest);
                var importedCard = imported.FirstOrDefault();
                
                if (importedCard?.Token != null)
                {
                    var assignNfcRequest = new AssignNfcCardRequest
                    {
                        Token = importedCard.Token
                    };
                    
                    await client.Users.AssignNfcCardToUserAsync(user.Id, assignNfcRequest);
                    Console.WriteLine($"✓ NFC card imported and assigned");
                }
            }
        }

        // Step 4: Generate PIN
        Console.WriteLine("\n[Step 4/4] Generate PIN Code");
        Console.WriteLine("------------------------");
        
        logger.LogInformation("Generating PIN code...");
        var pinCode = await client.Credentials.GeneratePinCodeAsync();
        
        var pinRequest = new AssignPinCodeRequest
        {
            PinCode = pinCode
        };
        
        await client.Users.AssignPinCodeToUserAsync(user.Id, pinRequest);
        Console.WriteLine($"✓ PIN code generated: {pinCode}");

        // Summary
        Console.WriteLine("\n=== Onboarding Complete! ===");
        Console.WriteLine($"User: {firstName} {lastName}");
        Console.WriteLine($"ID: {user.Id}");
        Console.WriteLine($"Email: {email ?? "N/A"}");
        Console.WriteLine($"Employee ID: {employeeId ?? "N/A"}");
        Console.WriteLine($"PIN Code: {pinCode}");
        Console.WriteLine("\n✓ User is ready for access!");
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
        var existingGroups = await client.UserGroups.GetUserGroupsAsync();
        
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