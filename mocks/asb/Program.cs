using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Core.Diagnostics;

public class ServiceBusEmulatorRefinedTester
{
    // Use the emulator's connection string or your Azure Service Bus connection string
    private const string ServiceBusEmulatorConnectionString = "your-service-bus-connection-string";

    // Specify your topic and subscription names
    private const string TestTopicName = "topic.1";
    private const string TestSubscriptionName = "subscription.1";

    private static readonly string[] EventsToFire = new string[]
    {
        "QMS.UserCertificate.Create.json"
        // Add more event files as needed
    };

    public static async Task Main(string[] args)
    {
        // _ = AzureEventSourceListener.CreateConsoleLogger(System.Diagnostics.Tracing.EventLevel.Verbose);

        Console.WriteLine("Attempting to connect to Service Bus Emulator with refined settings...");
        Console.WriteLine($"Using Connection String: {ServiceBusEmulatorConnectionString.Replace("SAS_KEY_VALUE", "[REDACTED FOR LOG, BUT USING THE LITERAL STRING]")}");

        ServiceBusClient client = null;

        try
        {
            var clientOptions = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpTcp // Crucial for non-TLS connection to port 5672
            };

            client = new ServiceBusClient(ServiceBusEmulatorConnectionString, clientOptions);

            // Send to topic
            Console.WriteLine($"Attempting to create a sender for topic '{TestTopicName}'...");
            ServiceBusSender sender = client.CreateSender(TestTopicName);

            Console.WriteLine("Sender created successfully. Attempting to send a message...");
            var eventsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "events");
            if (!Directory.Exists(eventsDirectory))
            {
                Console.WriteLine($"Events directory not found: {eventsDirectory}");
            }
            else
            {
                var eventFiles = Directory.GetFiles(eventsDirectory, "*.json");
                var matchedFiles = eventFiles.Where(file => EventsToFire.Contains(Path.GetFileName(file)));
                if (!matchedFiles.Any())
                {
                    Console.WriteLine("No matching event files found.");
                    return;
                }
                foreach (var file in matchedFiles)
                {
                    Console.WriteLine($"Sending message from file: {file}");
                    if (!File.Exists(file))
                    {
                        Console.WriteLine($"File not found: {file}");
                        continue;
                    }

                    string content = await File.ReadAllTextAsync(file);
                    var message = new ServiceBusMessage(System.Text.Encoding.UTF8.GetBytes(content))
                    {
                        ContentType = "application/json",
                        Subject = Path.GetFileName(file)
                    };
                    // Optionally set custom properties, e.g. eventType
                    message.ApplicationProperties["eventType"] = Path.GetFileNameWithoutExtension(file);

                    await sender.SendMessageAsync(message);
                    Console.WriteLine($"Sent message from file: {file}");
                }
            }
            Console.WriteLine("All messages sent!");

            await sender.CloseAsync();
            Console.WriteLine("Sender closed.");
        }
        catch (ServiceBusException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ServiceBusException: {ex.Message}");
            Console.WriteLine($"  Reason: {ex.Reason}");
            Console.WriteLine($"  IsTransient: {ex.IsTransient}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"  InnerException: {ex.InnerException.Message}");
                if (ex.InnerException.InnerException != null)
                {
                    Console.WriteLine($"    Inner-InnerException: {ex.InnerException.InnerException.Message}");
                }
            }
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"  InnerException: {ex.InnerException.Message}");
            }
            Console.ResetColor();
        }
        finally
        {
            if (client != null)
            {
                await client.DisposeAsync();
                Console.WriteLine("ServiceBusClient disposed.");
            }
        }

        Console.WriteLine("Test finished. Press any key to exit.");
        Console.ReadKey();
    }
}
