using EventManagmentTask.Models;

namespace EventManagmentTask.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(User user);
        RefreshToken GeneraterefreshToken();

    }
}
