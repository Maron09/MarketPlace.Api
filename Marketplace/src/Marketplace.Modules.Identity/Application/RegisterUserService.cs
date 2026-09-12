using Marketplace.Modules.Identity.Domain;
using Marketplace.SharedKernel;


namespace Marketplace.Modules.Identity.Application
{
    public sealed record RegisterUserRequest(string Email, string Password, UserRole Role);

    public sealed class RegisterUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        internal RegisterUserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<Guid>> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            if (await _userRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
            {
                return Result<Guid>.Failure("Email is already registered.", ErrorType.Conflict);
            }

            var passwordHash = _passwordHasher.Hash(request.Password);
            var user = User.Register(normalizedEmail, passwordHash, request.Role);
            await _userRepository.AddAsync(user, cancellationToken);

            return Result<Guid>.Success(user.Id);
        }
    }
}
