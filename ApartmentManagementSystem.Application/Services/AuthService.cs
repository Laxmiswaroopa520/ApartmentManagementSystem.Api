using ApartmentManagementSystem.Domain.Constants;
using ApartmentManagementSystem.Application.DTOs.Auth;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApartmentManagementSystem.Application.Services
{
    /// <summary>
    /// Service responsible for authentication operations.
    /// 
    /// Handles:
    /// - User login
    /// - Password verification
    /// - Account active status validation
    /// - JWT token generation
    /// </summary>
    public class AuthService : IAuthService
    {
        /// <summary>
        /// Repository used for retrieving user data.
        /// </summary>
        private readonly IUserRepository Users;

        /// <summary>
        /// Configuration used for accessing JWT settings.
        /// </summary>
        private readonly IConfiguration Config;

        /// <summary>
        /// Constructor for injecting dependencies.
        /// </summary>
        /// <param name="users">
        /// Repository responsible for user-related data access.
        /// </param>
        /// <param name="config">
        /// Application configuration containing JWT settings.
        /// </param>
        public AuthService(IUserRepository users, IConfiguration config)
        {
            Users = users;
            Config = config;
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token upon successful login.
        /// </summary>
        /// <param name="request">
        /// Login request containing username and password.
        /// </param>
        /// <returns>
        /// Login response containing JWT token and user details.
        /// </returns>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown when credentials are invalid or account is inactive.
        /// </exception>
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            // Retrieve user with roles
            var user = await Users.GetByUsernameWithRolesAsync(request.Username)
                ?? throw new UnauthorizedAccessException(ErrorMessages.InvalidCredentials);

            // Verify password using BCrypt hashing
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException(ErrorMessages.InvalidCredentials);

            // Check whether the account is active
            if (!user.IsActive)
                throw new UnauthorizedAccessException(ErrorMessages.AccountInactive);
                
            // Base claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName)
            };

            // Add role claims (one per role)
            foreach (var role in user.UserRoles.Select(ur => ur.Role.Name))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Generate signing key from configuration
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Config["JwtSettings:SecretKey"]!)
            );

            // Create JWT token
            var token = new JwtSecurityToken(
                issuer: Config["JwtSettings:Issuer"],
                audience: Config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                )
            );

            // Return response
            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserId = user.Id,
                FullName = user.FullName,

                // Primary role (for UI convenience)
                Role = user.UserRoles
                    .Select(ur => ur.Role.Name)
                    .FirstOrDefault()
            };
        }

        /// <summary>
        /// Checks whether a specific user account is active.
        /// </summary>
        /// <param name="userId">
        /// Unique identifier of the user.
        /// </param>
        /// <returns>
        /// True if the user exists and is active; otherwise false.
        /// </returns>
        public async Task<bool> IsUserActiveAsync(Guid userId)
        {
            var user = await Users.GetByIdAsync(userId);
            return user != null && user.IsActive;
        }
    }
}




















