using MediatR;

namespace QMS_Data_Updater.Domain.Events;

public class QMSUserCertificateCreateEvent : IRequest<IOperationResult>
{
    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets the certificate ID.
    /// </summary>
    public string CertificateId { get; set; }

    /// <summary>
    /// Gets or sets the certificate name.
    /// </summary>
    public string CertificateName { get; set; }

    /// <summary>
    /// Gets or sets the certificate type.
    /// </summary>
    public string CertificateType { get; set; }
}