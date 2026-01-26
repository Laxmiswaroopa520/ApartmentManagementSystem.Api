using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.Apartment
{
    public class FloorCreatedDto
    {
        public Guid FloorId { get; set; }
        public int FloorNumber { get; set; }
        public List<string> FlatNumbers { get; set; } = new();
    }
}
