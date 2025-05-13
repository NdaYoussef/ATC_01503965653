using EventManagmentTask.DTOs.UserDto;
using EventManagmentTask.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventManagmentTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        #region Constructor
        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        #endregion

        #region EndPoints

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _accountRepository.RegisterAsync(registerDto);
            if (response.IsSucceeded)
                return Ok(response);
            return StatusCode(response.StatusCode, new { response.Message });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _accountRepository.LoginAsync(loginDto);
            if (response.IsSucceeded)
                return Ok(response);
            return StatusCode(response.StatusCode, new { response.Message });
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] string email)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _accountRepository.Logout(email);
            if (response.IsSucceeded)
                return Ok(response);
            return StatusCode(response.StatusCode, new { response.Message });
        }

        [Authorize (Policy = "Admin")]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> NewRefreshToken([FromBody] string email)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _accountRepository.GetRefreshTokenAsync(email);
            if (response.IsSucceeded)
                return Ok(response);
            return StatusCode(response.StatusCode, new { response.Message });
        } 
        #endregion
    }
}
