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
    }

}
