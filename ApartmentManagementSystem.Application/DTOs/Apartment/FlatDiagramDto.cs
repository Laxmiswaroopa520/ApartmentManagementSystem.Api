using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.Apartment
{
    public class FlatDiagramDto
    {
        public Guid FlatId { get; set; }
        public string FlatNumber { get; set; } = string.Empty;
        public bool IsOccupied { get; set; }
        public string? OccupantName { get; set; }
        public string? OccupantType { get; set; } // Owner/Tenant
        public string Status { get; set; } = "Vacant"; // Vacant, Occupied, Reserved
    }
}