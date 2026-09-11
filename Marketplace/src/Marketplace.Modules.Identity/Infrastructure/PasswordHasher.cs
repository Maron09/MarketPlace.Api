using Marketplace.Modules.Identity.Application;
using Microsoft.AspNetCore.Identity;

namespace Marketplace.Modules.Identity.Application
{
    internal sealed class PasswordHasher : IPasswordHasher
    {
        // PasswordHasher<T> is generic over the "user" type only to namespace its
        // hash format internally — it doesn't actually touch our User class at all,
        // so a private marker object is enough; we never construct a real User here.
        private static readonly PasswordHasher<object> Underlying = new();

        public string Hash(string plainTextPassword)
            => Underlying.HashPassword(new object(), plainTextPassword);
        
        public bool Verify(string plainTextPassword, string hashedPassword)
        {
            var result = Underlying.VerifyHashedPassword(new object(), hashedPassword, plainTextPassword);
            return result is PasswordVerificationResult.Success
                or PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}