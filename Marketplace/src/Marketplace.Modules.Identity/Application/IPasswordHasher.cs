namespace Marketplace.Modules.Identity.Application
{
    internal interface IPasswordHasher
    {
        string Hash(string plainTextPassword);
        bool Verify (string plainTextPassword, string hashedPassword);
    }
}