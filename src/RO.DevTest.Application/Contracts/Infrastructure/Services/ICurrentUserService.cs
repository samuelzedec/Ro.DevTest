namespace RO.DevTest.Application.Contracts.Infrastructure.Services;

public interface ICurrentUserService
{
    Guid GetCurrentGuidId();
    bool IsAdmin();
}