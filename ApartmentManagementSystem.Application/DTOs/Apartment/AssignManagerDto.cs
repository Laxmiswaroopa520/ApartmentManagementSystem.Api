using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.Apartment
{
    public class AssignManagerDto
    {
        public Guid ApartmentId { get; set; }
        public Guid UserId { get; set; }
    }
}
