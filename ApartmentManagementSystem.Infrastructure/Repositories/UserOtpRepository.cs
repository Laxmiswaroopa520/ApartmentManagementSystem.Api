using System;

public class UserOtpRepository : IUserOtpRepository
{
    private readonly AppDbContext _db;

    public UserOtpRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(UserOtp otp)
    {
        _db.UserOtps.Add(otp);
        await _db.SaveChangesAsync();
    }

    public async Task<UserOtp?> GetValidOtpAsync(string email, string otp)
    {
        return await _db.UserOtps.FirstOrDefaultAsync(x =>
            x.Email == email &&
            x.Otp == otp &&
            !x.IsVerified &&
            x.ExpiresAt > DateTime.UtcNow);
    }

    public async Task UpdateAsync(UserOtp otp)
    {
        _db.UserOtps.Update(otp);
        await _db.SaveChangesAsync();
    }
}
