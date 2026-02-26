namespace ApartmentManagementSystem.Application.DTOs.Dashboard
{
    public class UpcomingEventDto
    {
        public Guid EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
