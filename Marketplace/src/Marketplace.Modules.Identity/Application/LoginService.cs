using Marketplace.Modules.Identity.Domain;

namespace Marketplace.Modules.Identity.Application
{
    public sealed record LoginRequest(string Email, string Password);

    public sealed record LoginResult(bool Succeeded, string? ErrorMessage, string? AccessToken);


    public sealed class LoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerator _tokenGenerator;


        internal LoginService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
        }


        public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var user = await _userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);

            if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                return new LoginResult(false, "Invalid email or password.", null);
            }

            if (!user.IsActive)
            {
                return new LoginResult(false, "Account is deactivated.", null);
            }

            var accessToken = _tokenGenerator.GenerateAccessToken(user.Id, user.Role);
            return new LoginResult(true, null, accessToken);
        }
    }
}