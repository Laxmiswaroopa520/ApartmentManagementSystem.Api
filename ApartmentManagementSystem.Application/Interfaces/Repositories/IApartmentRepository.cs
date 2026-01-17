using ApartmentManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
   
        public interface IApartmentRepository
        {
            Task<List<Apartment>> GetAllAsync();
            Task<Apartment?> GetByIdAsync(Guid id);
            Task<int> GetTotalCountAsync();

            Task AddAsync(Apartment apartment);
            Task SaveChangesAsync();
        }
    }



