namespace Marketplace.Modules.Identity.Domain
{
    internal sealed class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        private User() { }

        private User(Guid id, string email, string passwordHash, UserRole role)
        {
            Id = id;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            IsActive = true;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public static User Register(string email, string passwordHash, UserRole role)
            => new(Guid.NewGuid(), email, passwordHash, role);
        public void Deactivate() => IsActive = false;
    }

    internal enum UserRole { Customer, Vendor, Admin }
}