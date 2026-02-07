using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System;

namespace ApartmentManagementSystem.Application.DTOs.Manager
{
    public class AssignManagerRequestDto
    {
        public Guid ApartmentId { get; set; }

        // For existing resident-owner manager (internal)
        public Guid? UserId { get; set; }

        // For external manager (not a resident in the apartment)
        public bool IsExternalManager { get; set; } = false;
        public string? ExternalManagerName { get; set; }
        public string? ExternalManagerPhone { get; set; }
        public string? ExternalManagerEmail { get; set; }

        //Optional — does this manager live in this apartment?
        public bool LivesInApartment { get; set; } = false;
    }
}