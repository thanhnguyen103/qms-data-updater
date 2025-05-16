using MediatR;
using QMS_Data_Updater.Domain;
using QMS_Data_Updater.Domain.Events;

namespace QMS_Data_Updater.Application.Handlers;

public class QMSUserCertificateCreateEventHandler : BaseRequestHandler<QMSUserCertificateCreateEvent>
{
    private readonly IUserCertificateService _userCertificateService;

    public QMSUserCertificateCreateEventHandler(IUserCertificateService userCertificateService)
    {
        _userCertificateService = userCertificateService;
    }
    protected override async Task<IOperationResult> HandleRequest(QMSUserCertificateCreateEvent request, CancellationToken cancellationToken)
    {
        var result = await _userCertificateService.CreateCertificateAndRelatedDataAsync(request, cancellationToken);
        return result;
    }
}