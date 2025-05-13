using EventManagmentTask.DTOs;
using EventManagmentTask.DTOs.UserDto;

namespace EventManagmentTask.Interfaces
{
    public interface IAccountRepository
    {
        Task<ResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<ResponseDto> LoginAsync(LoginDto loginDto);
        Task<ResponseDto> Logout(string email);
        Task<ResponseDto> GetRefreshTokenAsync(string email);
    }
}
