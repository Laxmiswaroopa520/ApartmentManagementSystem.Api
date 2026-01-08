using System;

public class UserInviteRepository : IUserInviteRepository
{
    private readonly AppDbContext _db;

    public UserInviteRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(UserInvite invite)
    {
        _db.UserInvites.Add(invite);
        await _db.SaveChangesAsync();
    }

    public async Task<UserInvite?> GetValidInviteAsync(string email)
    {
        return await _db.UserInvites.FirstOrDefaultAsync(x =>
            x.Email == email &&
            !x.IsUsed &&
            x.ExpiresAt > DateTime.UtcNow);
    }

    public async Task UpdateAsync(UserInvite invite)
    {
        _db.UserInvites.Update(invite);
        await _db.SaveChangesAsync();
    }
}
