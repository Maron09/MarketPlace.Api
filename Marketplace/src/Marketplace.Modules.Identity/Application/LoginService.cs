using Marketplace.Modules.Identity.Domain;
using Marketplace.SharedKernel;
using Microsoft.Extensions.Logging;

namespace Marketplace.Modules.Identity.Application
{
    public sealed record LoginRequest(string Email, string Password);

    public sealed class LoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ILogger<LoginService> _logger;

        internal LoginService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator, ILogger<LoginService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
            _logger = logger;
        }

        public async Task<Result<string>> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);

            if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid Creentials");
                return Result<string>.Failure("Invalid email or password.", ErrorType.Unauthorized);
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Account is deactivated for email {Email}", normalizedEmail);
                return Result<string>.Failure("Account is deactivated.", ErrorType.Unauthorized);
            }

            var accessToken = _tokenGenerator.GenerateAccessToken(user.Id, user.Role);
            _logger.LogInformation("User {UserId} logged in successfully", user.Id);
            return Result<string>.Success(accessToken);
        }
    }
}