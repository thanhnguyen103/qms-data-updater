// Infrastructure Layer

using QMS_Data_Updater.Domain;
using QMS_Data_Updater.Domain.Events;
using QMS_Data_Updater.Infrastructure.Data;

public class UserCertificateService : IUserCertificateService
{
    private readonly AppDbContext _dbContext;

    public UserCertificateService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IOperationResult> CreateCertificateAndRelatedDataAsync(QMSUserCertificateCreateEvent evt, CancellationToken cancellationToken)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Update multiple tables
            // _dbContext.UserCertificates.Add(...);
            // _dbContext.OtherTable.Add(...);

            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            return OperationResult.Success("All updates succeeded");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return OperationResult.Failure("Failed to update all tables", exception: ex);
        }
    }
}