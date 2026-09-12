using Marketplace.Modules.Identity.Application;
using Marketplace.Modules.Identity.Domain;
using Marketplace.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;



namespace Marketplace.Modules.Identity.Api
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly RegisterUserService _registerUserService;
        private readonly LoginService _loginService;

        public AuthController(RegisterUserService registerUserService, LoginService loginService)
        {
            _registerUserService = registerUserService;
            _loginService = loginService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _registerUserService.RegisterAsync(
            new RegisterUserRequest(request.Email, request.Password, request.Role),
            cancellationToken);

            if (!result.IsSuccess)
                return this.ToActionResult(result);
            
            return StatusCode(StatusCodes.Status201Created, new { userId = result.Value });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _loginService.LoginAsync(new LoginRequest(request.Email, request.Password), cancellationToken);
            if (!result.IsSuccess)
                return this.ToActionResult(result);
            
            return Ok(new { accessToken = result.Value });
        }
    }


    public sealed record RegisterRequestDto(string Email, string Password, UserRole Role);
    public sealed record LoginRequestDto(string Email, string Password);
}