using ApartmentManagementSystem.Application.Interfaces.Services;

namespace ApartmentManagementSystem.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty");

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword) ||
                string.IsNullOrWhiteSpace(providedPassword))
                return false;

            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }
    }
}
