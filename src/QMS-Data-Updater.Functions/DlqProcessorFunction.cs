using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace QMS_Data_Updater.Functions;

public class DlqProcessorFunction
{
    private readonly ILogger<DlqProcessorFunction> _logger;

    public DlqProcessorFunction(ILogger<DlqProcessorFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(DlqProcessorFunction))]
    public async Task Run(
        [ServiceBusTrigger(
            "%ServiceBusTopicName%",
            "%ServiceBusSubscriptionName%/$DeadLetterQueue",
            Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogWarning("DLQ Message ID: {id}", message.MessageId);
        _logger.LogWarning("DLQ Message Body: {body}", message.Body);
        _logger.LogWarning("DLQ Message Content-Type: {contentType}", message.ContentType);

        // Optionally log DLQ properties
        foreach (var prop in message.ApplicationProperties)
        {
            _logger.LogWarning("DLQ Property: {key} = {value}", prop.Key, prop.Value);
        }

        // Complete the message to remove it from the DLQ
        await messageActions.CompleteMessageAsync(message);
    }
}