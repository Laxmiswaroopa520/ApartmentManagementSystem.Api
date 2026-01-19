using ApartmentManagementSystem.Domain.Entities.ApartmentManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IFloorRepository
    {
        Task<List<Floor>> GetAllAsync();
    }
}
