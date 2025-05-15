using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace QMS_Data_Updater.Functions;

public class UserCertificateEventHandler
{
    private readonly ILogger<UserCertificateEventHandler> _logger;

    public UserCertificateEventHandler(ILogger<UserCertificateEventHandler> logger)
    {
        _logger = logger;
    }

    [Function(nameof(UserCertificateEventHandler))]
    public async Task Run(
        [ServiceBusTrigger("mytopic", "mysubscription", Connection = "")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
        
    }
}