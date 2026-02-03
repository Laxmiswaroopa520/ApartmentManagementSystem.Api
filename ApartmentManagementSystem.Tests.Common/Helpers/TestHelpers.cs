// ApartmentManagementSystem.Tests.Common/Helpers/TestHelpers.cs
// CORRECTED VERSION - Fixed Random.Next long/int issue


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ApartmentManagementSystem.Tests.Common.Helpers
{
    public static class TestHelpers
    {
        /// <summary>
        /// Generate a JWT token for testing
        /// </summary>
        public static string GenerateJwtToken(
            Guid userId,
            string fullName,
            string role,
            /*string secretKey = "ThisIsAVerySecretKeyForTestingPurposesOnly12345",
              string issuer = "TestIssuer",
              string audience = "TestAudience")*/
            string secretKey = "ThisIsAVerySecretKeyForTestingPurposesOnly12345",
            string issuer = "TestIssuer",
            string audience = "TestAudience")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, fullName),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Create a random email address for testing
        /// </summary>
        public static string GenerateRandomEmail()
        {
            return $"test_{Guid.NewGuid():N}@example.com";
        }

        /// <summary>
        /// Create a random phone number for testing
        /// ✅ CORRECTED: Fixed long/int conversion issue
        /// </summary>
        public static string GenerateRandomPhone()
        {
            var random = new Random();

            // Option 1: Use smaller range that fits in int (recommended)
            return $"{random.Next(600000000, 999999999)}";  // 9-digit numbers

            // Option 2: If you need 10-digit numbers, use this approach:
            // return $"9{random.Next(100000000, 999999999)}";  // Starts with 9, then 9 more digits
        }

        /// <summary>
        /// Create a random phone number with specific format (10 digits)
        /// </summary>
        public static string GenerateRandomPhoneWithFormat()
        {
            var random = new Random();
            // Format: 9XXXXXXXXX (10 digits starting with 9)
            return $"9{random.Next(100000000, 999999999)}";
        }

        /// <summary>
        /// Create a random username for testing
        /// </summary>
        public static string GenerateRandomUsername()
        {
            return $"user_{Guid.NewGuid():N}".Substring(0, 20);
        }

        /// <summary>
        /// Wait for async operation with timeout
        /// </summary>
        public static async Task<T> WaitForAsync<T>(
            Func<Task<T>> operation,
            TimeSpan? timeout = null)
        {
            timeout ??= TimeSpan.FromSeconds(5);
            var cts = new CancellationTokenSource(timeout.Value);

            try
            {
                return await operation();
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"Operation timed out after {timeout.Value.TotalSeconds} seconds");
            }
        }

        /// <summary>
        /// Generate a random GUID string
        /// </summary>
        public static string GenerateRandomGuidString()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Create a random password that meets common requirements
        /// </summary>
        public static string GenerateRandomPassword()
        {
            var random = new Random();
            var lowercase = "abcdefghijklmnopqrstuvwxyz";
            var uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var digits = "0123456789";
            var special = "!@#$%^&*";

            // Ensure password has at least one of each required character type
            var password = new StringBuilder();
            password.Append(uppercase[random.Next(uppercase.Length)]);
            password.Append(lowercase[random.Next(lowercase.Length)]);
            password.Append(digits[random.Next(digits.Length)]);
            password.Append(special[random.Next(special.Length)]);

            // Fill the rest with random characters
            var allChars = lowercase + uppercase + digits + special;
            for (int i = 0; i < 8; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Shuffle the password
            return new string(password.ToString().OrderBy(_ => random.Next()).ToArray());
        }
    }
}

/*    // ApartmentManagementSystem.Tests.Common/Helpers/TestHelpers.cs
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApartmentManagementSystem.Tests.Common.Helpers
{
    public static class TestHelpers
    {
        // ⚠️ CRITICAL: This must match your appsettings.json JWT settings EXACTLY
        private const string SECRET_KEY = "YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256Algorithm!";
        private const string ISSUER = "ApartmentManagementSystem";
        private const string AUDIENCE = "ApartmentManagementSystemUsers";

        public static string GenerateJwtToken(Guid userId, string fullName, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, fullName),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: ISSUER,
                audience: AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
*/