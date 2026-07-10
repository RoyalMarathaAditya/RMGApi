namespace HRMS.Api.Services.Interfaces.UserManagement
{
    public interface IUserSynchronizationService
    {
        Task SyncEmployeesAsync(CancellationToken cancellationToken = default);
    }
}
