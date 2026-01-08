using System;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(string email, string password);
}
