using System;
using System.Collections.Generic;
/*
namespace ApartmentManagementSystem.Domain.Entities
{
       public class Flat
       {
           public Guid Id { get; set; }

           public string FlatNumber { get; set; } = string.Empty;

           // =========================
           // FOREIGN KEYS
           // =========================
           public Guid ApartmentId { get; set; }
           public Guid? OwnerUserId { get; set; }
           public Guid? TenantUserId { get; set; }

           // =========================
           // NAVIGATION PROPERTIES
           // =========================
           public Apartment Apartment { get; set; } = null!;

           public User? OwnerUser { get; set; }
           public User? TenantUser { get; set; }

           public ICollection<UserFlatMapping> UserFlatMappings { get; set; }
               = new List<UserFlatMapping>();

           // =========================
           // SYSTEM FIELDS
           // =========================
           public bool IsActive { get; set; }

           public DateTime CreatedAt { get; set; }
           public DateTime? UpdatedAt { get; set; }
       }
   }
   */

    using System;
    using System.Collections.Generic;

    namespace ApartmentManagementSystem.Domain.Entities
    {
        public class Flat
        {
            public Guid Id { get; set; }

            public string FlatNumber { get; set; } = string.Empty;

            // =========================
            // FOREIGN KEYS
            // =========================
            public Guid ApartmentId { get; set; }
            public Guid? OwnerUserId { get; set; }   // ✔ Only ONE User FK

            // =========================
            // NAVIGATION PROPERTIES
            // =========================
            public Apartment Apartment { get; set; } = null!;

            public User? OwnerUser { get; set; }

            // Tenants come via mapping table
            public ICollection<UserFlatMapping> UserFlatMappings { get; set; }
                = new List<UserFlatMapping>();

            // =========================
            // SYSTEM FIELDS
            // =========================
            public bool IsActive { get; set; }

            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    }








/*  public class Flat
{
    public Guid Id { get; set; }

    public string FlatNumber { get; set; } = string.Empty;

    // =========================
    // APARTMENT RELATION
    // =========================
    public Guid ApartmentId { get; set; }
    public Apartment Apartment { get; set; } = null!;

    // =========================
    // OWNER RELATION
    // =========================
    public Guid? OwnerUserId { get; set; }
    public User? OwnerUser { get; set; }

    // =========================
    // TENANT RELATION
    // =========================
    public Guid? TenantUserId { get; set; }
    public User? TenantUser { get; set; }

    // =========================
    // STATUS & AUDIT
    // =========================
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // =========================
    // MAPPING (HISTORY / ROLES)
    // =========================
    public ICollection<UserFlatMapping> UserFlatMappings { get; set; }
        = new List<UserFlatMapping>();
}
*/
