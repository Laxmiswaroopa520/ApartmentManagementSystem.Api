using ApartmentManagementSystem.Application.DTOs.Auth;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
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
    /// - User login with credential validation
    /// - BCrypt password verification
    /// - Account active status check
    /// - JWT token generation with role claims
    /// </summary>
    public class AuthService : IAuthService
    {
        /// <summary>Unit of Work providing access to all repositories.</summary>
        private readonly IUnitOfWork UoW;

        /// <summary>Application configuration used for JWT settings.</summary>
        private readonly IConfiguration Config;

        /// <summary>
        /// Initialises AuthService with required dependencies.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work for data access.</param>
        /// <param name="config">Configuration containing JWT secret, issuer, and audience.</param>
        public AuthService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            UoW = unitOfWork;
            Config = config;
        }

        /// <summary>
        /// Authenticates a user and generates a signed JWT token on success.
        ///
        /// Steps:
        /// 1. Retrieves user with roles by username.
        /// 2. Verifies password using BCrypt.
        /// 3. Confirms account is active.
        /// 4. Builds claims (NameIdentifier, Name, one Role claim per role).
        /// 5. Signs and returns a 24-hour JWT.
        /// </summary>
        /// <param name="request">Login credentials — username and password.</param>
        /// <returns>Login response containing JWT token, UserId, FullName, and primary Role.</returns>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown when username is not found, password is incorrect, or account is inactive.
        /// </exception>
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await UoW.Users.GetByUsernameWithRolesAsync(request.Username)
                ?? throw new UnauthorizedAccessException(ErrorMessages.InvalidCredentials);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException(ErrorMessages.InvalidCredentials);

            if (!user.IsActive)
                throw new UnauthorizedAccessException(ErrorMessages.AccountInactive);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.FullName)
            };

            foreach (var role in user.UserRoles.Select(ur => ur.Role.Name))
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["JwtSettings:SecretKey"]!));
            var token = new JwtSecurityToken(
                issuer: Config["JwtSettings:Issuer"],
                audience: Config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserId = user.Id,
                FullName = user.FullName,
                Role = user.UserRoles.Select(ur => ur.Role.Name).FirstOrDefault()
            };
        }

        /// <summary>
        /// Checks whether a specific user account is currently active.
        /// </summary>
        /// <param name="userId">Unique identifier of the user to check.</param>
        /// <returns>
        /// <c>true</c> if the user exists and IsActive is true; otherwise <c>false</c>.
        /// </returns>
        public async Task<bool> IsUserActiveAsync(Guid userId)
        {
            var user = await UoW.Users.GetByIdAsync(userId);
            return user != null && user.IsActive;
        }
    }
}
























/*using ApartmentManagementSystem.Application.DTOs.Auth;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApartmentManagementSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork UoW;
        private readonly IConfiguration Config;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            UoW = unitOfWork;
            Config = config;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await UoW.Users.GetByUsernameWithRolesAsync(request.Username)
                ?? throw new UnauthorizedAccessException(ErrorMessages.InvalidCredentials);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException(ErrorMessages.InvalidCredentials);

            if (!user.IsActive)
                throw new UnauthorizedAccessException(ErrorMessages.AccountInactive);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.FullName)
            };

            foreach (var role in user.UserRoles.Select(ur => ur.Role.Name))
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["JwtSettings:SecretKey"]!));
            var token = new JwtSecurityToken(
                issuer: Config["JwtSettings:Issuer"],
                audience: Config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserId = user.Id,
                FullName = user.FullName,
                Role = user.UserRoles.Select(ur => ur.Role.Name).FirstOrDefault()
            };
        }

        public async Task<bool> IsUserActiveAsync(Guid userId)
        {
            var user = await UoW.Users.GetByIdAsync(userId);
            return user != null && user.IsActive;
        }
    }
}

*/





























































