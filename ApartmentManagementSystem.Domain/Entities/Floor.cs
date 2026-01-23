namespace ApartmentManagementSystem.Domain.Entities
{
        public class Floor
        {
            public Guid Id { get; set; }
            public int FloorNumber { get; set; }

            public Guid ApartmentId { get; set; }
            public Apartment Apartment { get; set; } = null!;
            public string Name { get; set; } = null!;
            public ICollection<Flat> Flats { get; set; } = new List<Flat>();
        }
    }

