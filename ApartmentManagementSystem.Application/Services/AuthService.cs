using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IJwtTokenGenerator _jwtGenerator;

        public AuthService(
            IUserRepository userRepo,
            IJwtTokenGenerator jwtGenerator)
        {
            _userRepo = userRepo;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<LoginResponse> LoginAsync(string email, string password)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null)
                throw new Exception("Invalid credentials");

            if (!user.IsActive || !user.IsRegistrationCompleted)
                throw new Exception("Registration incomplete");

            var valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!valid)
                throw new Exception("Invalid credentials");

            var token = _jwtGenerator.Generate(user);

            return new LoginResponse { Token = token };
        }
    }
}