namespace Marketplace.Modules.Identity.Application
{
    public interface IPasswordHasher
    {
        string Hash(string plainTextPassword);
        bool Verify (string plainTextPassword, string hashedPassword);
    }
}