using System.ComponentModel.DataAnnotations;

namespace ApartmentManagementSystem.Domain.Enums
{
  public  enum ResidentType
    {
        [Display(Name = "Resident Owner")]
        Owner = 1,
        Tenant = 2,
        Staff = 3
    }
}