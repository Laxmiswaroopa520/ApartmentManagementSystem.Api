using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.Community.Resident_Management
{

    public class ResidentDetailDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PrimaryPhone { get; set; } = string.Empty;
        public string? SecondaryPhone { get; set; }
        public string ResidentType { get; set; } = string.Empty;
        public string? FlatNumber { get; set; }
        public string? ApartmentName { get; set; }
        public DateTime RegisteredOn { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public int TotalComplaints { get; set; }
        public decimal OutstandingBills { get; set; }
    }

}
