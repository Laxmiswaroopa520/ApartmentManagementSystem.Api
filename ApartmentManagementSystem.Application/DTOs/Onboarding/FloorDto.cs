using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ApartmentManagementSystem.Application.DTOs.Onboarding
{
    public class FloorDto
    {
        public Guid Id { get; set; }
        public int FloorNumber { get; set; }
        public Guid ApartmentId { get; set; } // ⭐ Add this
        public string ApartmentName { get; set; } = string.Empty; // ⭐ Add this
    }
}


/*
namespace ApartmentManagementSystem.Application.DTOs.Onboarding
{
    public class FloorDto
    {
        public Guid Id { get; set; }
        public int FloorNumber { get; set; }
    }

}
*/