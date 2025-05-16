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
            Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        try
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            if (!message.ApplicationProperties.TryGetValue("eventType", out var eventTypeObj) ||
                eventTypeObj is not string eventType)
            {
                _logger.LogError("Missing or invalid eventType property.");
                await messageActions.DeadLetterMessageAsync(message, new Dictionary<string, object>
                {
                    { "DeadLetterReason", "MissingEventType" },
                    { "DeadLetterErrorDescription", "The eventType property is missing or invalid." }
                });
                return;
            }

            var payload = message.Body.ToString();

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