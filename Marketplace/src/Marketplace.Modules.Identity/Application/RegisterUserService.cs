using Marketplace.Modules.Identity.Domain;
using Marketplace.SharedKernel;
using Microsoft.Extensions.Logging;


namespace Marketplace.Modules.Identity.Application
{
    public sealed record RegisterUserRequest(string Email, string Password, UserRole Role);

    public sealed class RegisterUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<RegisterUserService> _logger;

        internal RegisterUserService(IUserRepository userRepository, IPasswordHasher passwordHasher, ILogger<RegisterUserService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<Result<Guid>> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            if (request.Role == UserRole.Admin)
            {
                _logger.LogWarning("Rejected self-registration attempt with Admin role for email {Email}", request.Email);
                return Result<Guid>.Failure("Cannot self-register as Admin.", ErrorType.Validation);
            }
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            if (await _userRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
            {
                _logger.LogInformation("Registration rejected: email {Email} already registered", normalizedEmail);
                return Result<Guid>.Failure("Email is already registered.", ErrorType.Conflict);
            }

            var passwordHash = _passwordHasher.Hash(request.Password);
            var user = User.Register(normalizedEmail, passwordHash, request.Role);
            await _userRepository.AddAsync(user, cancellationToken);

            _logger.LogInformation("User {UserId} registered with role {Role}", user.Id, request.Role);
            return Result<Guid>.Success(user.Id);
        }
    }
}
