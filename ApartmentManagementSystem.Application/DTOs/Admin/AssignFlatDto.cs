using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.Admin
{
  public  class AssignFlatDto
    {
        public Guid UserId { get; set; }
        public Guid FlatId { get; set; }
    }
}
