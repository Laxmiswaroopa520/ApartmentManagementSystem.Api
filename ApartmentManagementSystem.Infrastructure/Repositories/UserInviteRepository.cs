using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    using ApartmentManagementSystem.Application.Interfaces.Repositories;
    using ApartmentManagementSystem.Domain.Entities;
    using ApartmentManagementSystem.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;

    //   using global::ApartmentManagementSystem.Application.Interfaces.Repositories;
    // using global::ApartmentManagementSystem.Domain.Entities;
    //   using global::ApartmentManagementSystem.Infrastructure.Persistence;

    //  namespace ApartmentManagementSystem.Infrastructure.Repositories;

    /*  public class UserInviteRepository : IUserInviteRepository
      {
          private readonly AppDbContext _db;
          public UserInviteRepository(AppDbContext db) => _db = db;

          public async Task AddAsync(UserInvite invite)
              => await _db.UserInvites.AddAsync(invite);
      }*/



    public class UserInviteRepository : IUserInviteRepository
    {
        private readonly AppDbContext _context;

        public UserInviteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserInvite?> GetByPhoneAsync(string phone)
        {
            return await _context.UserInvites
                .FirstOrDefaultAsync(i => i.PrimaryPhone == phone);
        }

        public async Task AddAsync(UserInvite invite)
        {
            await _context.UserInvites.AddAsync(invite);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}