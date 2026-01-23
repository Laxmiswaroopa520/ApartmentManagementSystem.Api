//using ApartmentManagementSystem.Application.Interfaces.Repositories;
//using ApartmentManagementSystem.Application.Interfaces.Services;



using ApartmentManagementSystem.Application.DTOs.Auth;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApartmentManagementSystem.Application.Services;

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
        var user = await Users.GetByUsernameWithRolesAsync(request.Username)
            ?? throw new UnauthorizedAccessException("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        //  STATUS CHECK  here like whether the user is active or inactive..
        if (!user.IsActive)   // or user.Status != UserStatus.Active
            throw new UnauthorizedAccessException(
                "Your account is inactive. Please contact the administrator."
            );

        // Base claims (UNCHANGED)
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.FullName)
    };

        // Add ONE role claim per role (UNCHANGED)
        foreach (var role in user.UserRoles.Select(ur => ur.Role.Name))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

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

            // Primary role (UI convenience) (UNCHANGED)
            Role = user.UserRoles
                .Select(ur => ur.Role.Name)
                .FirstOrDefault()
        };
    }
    public async Task<bool> IsUserActiveAsync(Guid userId)
    {
        var user = await Users.GetByIdAsync(userId);
        return user != null && user.IsActive;
    }
}


    /*  public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
      {
          var user = await Users.GetByUsernameWithRolesAsync(request.Username)
              ?? throw new UnauthorizedAccessException("Invalid credentials");

          if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
              throw new UnauthorizedAccessException("Invalid credentials");

          // Base claims
          var claims = new List<Claim>
          {
              new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
              new Claim(ClaimTypes.Name, user.FullName)
          };

          // Add ONE role claim per role
          foreach (var role in user.UserRoles.Select(ur => ur.Role.Name))
          {
              claims.Add(new Claim(ClaimTypes.Role, role));
          }

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

              //  Primary role (UI convenience)
              Role = user.UserRoles
                  .Select(ur => ur.Role.Name)
                  .FirstOrDefault()
          };*/


























/*
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
*/