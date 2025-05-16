using System.Text.Json;
using Azure.Messaging.ServiceBus;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QMS_Data_Updater.Domain;
using QMS_Data_Updater.Domain.Events;

namespace QMS_Data_Updater.Functions;

public class MainMessageProcessorFunction
{
    private readonly ILogger<MainMessageProcessorFunction> _logger;
    private readonly IMediator _mediator;

    public MainMessageProcessorFunction(ILogger<MainMessageProcessorFunction> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [Function(nameof(MainMessageProcessorFunction))]
    public async Task Run(
        [ServiceBusTrigger(
            "%ServiceBusTopicName%",
            "%ServiceBusSubscriptionName%",
            Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        try
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);

            // Get the message body as a string
            string payload = message.Body.ToString();

            // Parse the message body as JSON to extract eventType from content.eventType
            using var doc = JsonDocument.Parse(payload);
            if (!doc.RootElement.TryGetProperty("content", out var contentElement) ||
                !contentElement.TryGetProperty("eventType", out var eventTypeElement) ||
                eventTypeElement.ValueKind != JsonValueKind.String)
            {
                _logger.LogError("Missing or invalid eventType property in message body content.");
                await messageActions.DeadLetterMessageAsync(message, new Dictionary<string, object>
                {
                    { "DeadLetterReason", "MissingEventType" },
                    { "DeadLetterErrorDescription", "The eventType property is missing or invalid in the message body content." }
                });
                return;
            }

            string eventType = eventTypeElement.GetString()!;

            IRequest<IOperationResult>? request = eventType switch
            {
                "QMS.UserCertificate.Create" => JsonSerializer.Deserialize<QMSUserCertificateCreateEvent>(payload),
                // ...more mappings
                _ => null
            };

            if (request == null)
            {
                _logger.LogError("Unknown or unsupported event type: {eventType}", eventType);
                await messageActions.DeadLetterMessageAsync(message, new Dictionary<string, object>
                {
                    { "DeadLetterReason", "UnknownEventType" },
                    { "DeadLetterErrorDescription", $"Unknown event type: {eventType}" }
                });
                return;
            }

            var response = await _mediator.Send(request);

            if (response is { } opResult && !opResult.IsSuccess)
            {
                _logger.LogError("Handler failed to update the database: {reason}", opResult.Message);
                await messageActions.AbandonMessageAsync(message);
                return;
            }

            await messageActions.CompleteMessageAsync(message);
        }
        catch (DbUpdateConcurrencyException ex) // Example: Optimistic concurrency, might be retryable
        {
            _logger.LogWarning(ex, "Database concurrency issue for message {MessageId}. Abandoning for retry.",
                message.MessageId);
            await messageActions.AbandonMessageAsync(message); // Let Service Bus retry
        }
        catch (SqlException sqlEx) when (IsTransient(sqlEx)) // IsTransient is a helper you'd write
        {
            _logger.LogWarning(sqlEx, "Transient SQL error for message {MessageId}. Abandoning for retry.",
                message.MessageId);
            await messageActions.AbandonMessageAsync(message); // Let Service Bus retry
        }
        catch (Exception ex) // For other, potentially non-transient DB errors or critical issues
        {
            _logger.LogError(ex, "Failed to update database for message {MessageId}. Sending to DLQ.",
                message.MessageId);
            await messageActions.DeadLetterMessageAsync(message, new Dictionary<string, object>
            {
                { "DeadLetterReason", "DatabaseUpdateFailure" },
                { "OriginalExceptionType", ex.GetType().ToString() },
                { "OriginalExceptionMessage", ex.Message }
            });
        }

        if (message.DeliveryCount > 5) // for example, limit to 5 attempts
        {
            _logger.LogWarning("Message {MessageId} exceeded max delivery attempts. Sending to DLQ.", message.MessageId);
            await messageActions.DeadLetterMessageAsync(message, new Dictionary<string, object>
            {
                { "DeadLetterReason", "MaxDeliveryAttemptsExceeded" },
                { "DeadLetterErrorDescription", "The message exceeded the maximum number of delivery attempts." }
            });
            return;
        }
    }
    
    private bool IsTransient(SqlException ex)
    {
        // Add specific SQL error numbers that indicate transient failures
        // These numbers can vary by database (SQL Server, MySQL, Postgre, etc.)
        // Common examples for SQL Server:
        // 40613: Database unavailable
        // 40501: Service busy
        // 40197: The service is currently busy
        // -2:    Timeout expired
        // 1205:  Deadlock
        int[] transientErrorNumbers = { 40613, 40501, 40197, -2, 1205 /*...and others...*/ };
        return transientErrorNumbers.Contains(ex.Number);
    }
}