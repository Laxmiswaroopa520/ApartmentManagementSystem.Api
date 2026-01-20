using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Add  ResidentType instead of FlatId
namespace ApartmentManagementSystem.Application.DTOs.Onboarding
{
    // namespace ApartmentManagementSystem.Application.DTOs.Onboarding;

   public  class CreateUserInviteDto
    {
        public string FullName { get; set; } = string.Empty;
        public string PrimaryPhone { get; set; } = string.Empty;
        //  public Guid RoleId { get; set; }
        public int ResidentType { get; set; }
        
       // 1=Owner, 2=Tenant, 3=Staff
    }
}