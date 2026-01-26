using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ApartmentManagementSystem.Application.DTOs.Onboarding
{
    public class FlatDto
    {
        public Guid Id { get; set; }
        public string FlatNumber { get; set; } = string.Empty;
        public Guid FloorId { get; set; }
        public Guid ApartmentId { get; set; } // ⭐ Add this
        public bool IsOccupied { get; set; }
    }
}
/*
namespace ApartmentManagementSystem.Application.DTOs.Onboarding
{
    public class FlatDto
    {
        public Guid Id { get; set; }
        public string FlatNumber { get; set; } = string.Empty;
    }

}
*/
