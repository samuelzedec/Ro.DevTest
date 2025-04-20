namespace RO.DevTest.Application.Contracts.Infrastructure.Services;

public interface ICurrentUserService
{
    string GetCurrentUserId();
    bool IsAdmin();
}