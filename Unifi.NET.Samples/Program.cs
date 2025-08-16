using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Unifi.NET.Access;
using Unifi.NET.Access.Configuration;
using Unifi.NET.Access.Models.Credentials;
using Unifi.NET.Access.Models.Users;

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
    Console.WriteLine("UniFi Access SDK Sample - User and NFC Card Management");
    Console.WriteLine("========================================================\n");

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
        Console.WriteLine("\n--- Main Menu ---");
        Console.WriteLine("1. Create new user");
        Console.WriteLine("2. List all users");
        Console.WriteLine("3. Register NFC card for user");
        Console.WriteLine("4. Assign PIN code to user");
        Console.WriteLine("5. List devices");
        Console.WriteLine("6. Exit");
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
                await RegisterNfcCard(client, logger);
                break;
            case "4":
                await AssignPinCode(client, logger);
                break;
            case "5":
                await ListDevices(client, logger);
                break;
            case "6":
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

async Task RegisterNfcCard(IUnifiAccessClient client, ILogger logger)
{
    Console.WriteLine("\n--- Register NFC Card ---");
    
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
        Console.Write("\nEnter a name for this card (e.g., 'Office Card'): ");
        var cardName = Console.ReadLine()?.Trim() ?? "NFC Card";
        
        logger.LogInformation("Assigning NFC card to user...");
        var assignRequest = new AssignNfcCardRequest
        {
            Token = detectedCardId
        };
        
        await client.Users.AssignNfcCardToUserAsync(userId, assignRequest);
        
        Console.WriteLine($"\n✓ NFC card successfully assigned to user!");
        Console.WriteLine($"  Card Token: {detectedCardId}");
        Console.WriteLine($"  Card Name: {cardName}");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to register NFC card");
        Console.WriteLine($"Error registering NFC card: {ex.Message}");
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