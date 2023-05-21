using Microsoft.AspNetCore.Identity;
using HousingPortalApi.Models;

namespace HousingPortalApi.Interfaces
{
    public interface IPasswordHasherWrapper
    {
        string HashPassword(HousingPortalUser user, string password);
        PasswordVerificationResult VerifyHashedPassword(HousingPortalUser user, string hashedPassword, string providedPassword);
    }

    public class PasswordHasherWrapper : IPasswordHasherWrapper
    {
        private readonly PasswordHasher<HousingPortalUser> _passwordHasher;

        public PasswordHasherWrapper(PasswordHasher<HousingPortalUser> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public string HashPassword(HousingPortalUser user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public PasswordVerificationResult VerifyHashedPassword(HousingPortalUser user, string hashedPassword, string providedPassword)
        {
            return _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        }
    }
}
