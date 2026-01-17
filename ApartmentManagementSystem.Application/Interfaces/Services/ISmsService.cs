using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Services
{
     public interface ISmsService
        {
            Task SendAsync(string phone, string message);
        }
    }
