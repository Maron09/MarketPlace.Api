using Marketplace.Modules.Identity.Domain;
using Marketplace.SharedKernel;

namespace Marketplace.Modules.Identity.Application
{
    public sealed record LoginRequest(string Email, string Password);

    public sealed class LoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerator _tokenGenerator;

        internal LoginService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<Result<string>> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);

            if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                return Result<string>.Failure("Invalid email or password.", ErrorType.Unauthorized);
            }

            if (!user.IsActive)
            {
                return Result<string>.Failure("Account is deactivated.", ErrorType.Unauthorized);
            }

            var accessToken = _tokenGenerator.GenerateAccessToken(user.Id, user.Role);
            return Result<string>.Success(accessToken);
        }
    }
}