using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
   // public ICollection<User> Users { get; set; } = new List<User>();
    // this collection is for having 1 user has multiple roles..
    // public ICollection<RoleNames> UserRoles { get; set; } = new List<RoleNames>();
    //Enusures many to many via UserRole

    //because 1 user can have many roles as well as 1 role can have many users..


    //1-user many roles and also many users are tenants,security like that ..
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}