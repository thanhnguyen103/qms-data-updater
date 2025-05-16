using System;

namespace QMS_Data_Updater.Domain.Entities;

public class UserCertificate
{
    public Guid UserCertificateGUID { get; set; }
    public Guid CertificateGUID { get; set; }
    public Guid UserGUID { get; set; }
    public string? LicenseNumber { get; set; }
    public string? IssuingCountry { get; set; }
    public string? Grade { get; set; }
    public string? Trainer { get; set; }
    public string? Hours { get; set; }
    public bool RequireApproval { get; set; }
    public bool IsOfficial { get; set; }
    public string? Notes { get; set; }
    public string? TimeZone { get; set; }
    public string? SuspensionType { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsLatest { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public DateTime LastUpdatedDate { get; set; }

    public UserCertificate()
    {
        RequireApproval = false;
        IsOfficial = false;
        IsLatest = false;
        CreatedDate = TimeProvider.System.GetUtcNow().UtcDateTime;
        LastUpdatedDate = TimeProvider.System.GetUtcNow().UtcDateTime;
    }
}