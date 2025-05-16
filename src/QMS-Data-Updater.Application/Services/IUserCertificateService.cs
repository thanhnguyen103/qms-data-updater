// Application Layer

using QMS_Data_Updater.Domain;
using QMS_Data_Updater.Domain.Events;

public interface IUserCertificateService
{
    Task<IOperationResult> CreateCertificateAndRelatedDataAsync(QMSUserCertificateCreateEvent evt, CancellationToken cancellationToken);
}