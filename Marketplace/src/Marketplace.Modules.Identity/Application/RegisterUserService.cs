using Marketplace.Modules.Identity.Domain;


namespace Marketplace.Modules.Identity.Application
{
    public sealed record RegisterUserRequest(string Email, string Password, UserRole Role);

    public sealed record RegisterUserResult(bool Succeeded, string? ErrorMessage, Guid? UserId);

    public sealed class RegisterUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        internal RegisterUserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<RegisterUserResult>RegisterAsync(
            RegisterUserRequest request,
            CancellationToken cancellationToken)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            if (await _userRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
            {
                return new RegisterUserResult(false, "Email is already registered.", null);
            }
            var passwordHash = _passwordHasher.Hash(request.Password);
            var user = User.Register(normalizedEmail, passwordHash, request.Role);

            await _userRepository.AddAsync(user, cancellationToken);
            return new RegisterUserResult(true, null, user.Id);
        }
    }
}