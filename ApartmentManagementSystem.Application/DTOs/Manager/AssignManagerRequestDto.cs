using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.Manager
{
    public class AssignManagerRequestDto
    {
        public Guid ApartmentId { get; set; }
        public Guid UserId { get; set; }
    }
}