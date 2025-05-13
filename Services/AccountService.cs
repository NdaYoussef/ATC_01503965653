using EventManagmentTask.Data;
using EventManagmentTask.DTOs;
using EventManagmentTask.DTOs.UserDto;
using EventManagmentTask.Interfaces;
using EventManagmentTask.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace EventManagmentTask.Services
{
    public class AccountService : IAccountRepository
    {
        private readonly EventManagmentDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public AccountService(EventManagmentDbContext context , UserManager<User> userManager, ITokenService tokenService) : base()
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<ResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            if (await _userManager.FindByEmailAsync(registerDto.Email) is not null
             || await _userManager.FindByNameAsync(registerDto.UserName) is not null)
            {
                return new ResponseDto
                {
                    Message = "Email or User Name already exists!!",
                    IsSucceeded = false,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            var user = registerDto.Adapt<User>();
            user.IsConfirmed = true;


            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new ResponseDto
                {
                    Message = errors,
                    IsSucceeded = false,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            // Assign the specified role
            await _userManager.AddToRoleAsync(user, registerDto.Role.ToString());

            return new ResponseDto
            {
                Message = "User registered successfully.",
                IsSucceeded = true,
                StatusCode = (int)HttpStatusCode.OK,
                Data = new
                {
                    UserName = user.UserName,
                    Email = user.Email
                }
            };
        }

        public async Task<ResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return new ResponseDto
                {
                    Message = "User not found!!",
                    IsSucceeded = false,
                    StatusCode = 400
                };
            }
            if (user.Role != loginDto.Role)
                return new ResponseDto
                {
                    Message = "Role doesnt match, try again please",
                    IsSucceeded = false,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            var token = await _tokenService.GenerateToken(user);
            var refreshToken = string.Empty;
            DateTime refreshTokenExpiration;
            if (user.RefreshTokens!.Any(t => t.IsActive))
            {
                var activeToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                refreshToken = activeToken.Token;
                refreshTokenExpiration = activeToken.ExpiresOn;
            }
            else
            {
                var newRefreshToken = _tokenService.GeneraterefreshToken();
                refreshToken = newRefreshToken.Token;
                refreshTokenExpiration = newRefreshToken.ExpiresOn;
            }
            return new ResponseDto
            {
                Message = " User login successfully ",
                IsSucceeded = true,
                StatusCode = 200,
                Data = new
                {
                    IsAuthenticated = true,
                    token = token,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiration = refreshTokenExpiration,
                    UserName = user.UserName,
                    Email = user.Email
                }
            };
        }

        public async Task<ResponseDto> Logout(string email)
        {
            var user = await _userManager.Users
         .Include(u => u.RefreshTokens)
         .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return new ResponseDto
                {
                    Message = "User not found",
                    IsSucceeded = false,
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };
            }

            var activeTokens = user.RefreshTokens?
                .Where(t => t.IsActive)
                .ToList();

            if (activeTokens != null && activeTokens.Any())
            {
                foreach (var token in activeTokens)
                {
                    token.RevokedOn = DateTime.UtcNow;
                }

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return new ResponseDto
                    {
                        Message = "Failed to logout",
                        IsSucceeded = false,
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                }
            }

            return new ResponseDto
            {
                Message = "Logged out successfully",
                IsSucceeded = true,
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        public async Task<ResponseDto> GetRefreshTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ResponseDto
                {
                    Message = "Invalid Email!!",
                    IsSucceeded = false,
                    StatusCode = 400,
                };
            }
            var activeToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
            if (activeToken is not null)
            {
                return new ResponseDto
                {
                    Message = "Token still active",
                    IsSucceeded = false,
                    StatusCode = 400,
                };
            }
            var token = await _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GeneraterefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);
            return new ResponseDto
            {
                IsSucceeded = true,
                StatusCode = 200,
                Data = new
                {
                    IsAuthenticated = true,
                    //       Token = token,
                    RefreshToken = refreshToken,
                    UserName = user.UserName,
                    Email = user.Email,
                }
            };
        }

    }
}
