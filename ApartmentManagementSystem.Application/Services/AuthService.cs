//using ApartmentManagementSystem.Application.Interfaces.Repositories;
//using ApartmentManagementSystem.Application.Interfaces.Services;

namespace ApartmentManagementSystem.Application.Services
{
    using ApartmentManagementSystem.Application.DTOs.Auth;
    using ApartmentManagementSystem.Application.Interfaces.Repositories;
    using ApartmentManagementSystem.Application.Interfaces.Services;
  //  using global::ApartmentManagementSystem.Application.DTOs.Auth;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
 

    //namespace ApartmentManagementSystem.Application.Services;

    public class AuthService : IAuthService
    {
        private readonly IUserRepository Users;
        private readonly IConfiguration Config;

        public AuthService(IUserRepository users, IConfiguration config)
        {
            Users = users;
            Config = config;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await Users.GetByUsernameAsync(request.Username)
                ?? throw new UnauthorizedAccessException("Invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Config["JwtSettings:SecretKey"]!)
            );

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
                Role = user.Role.Name
            };
        }
    }
}