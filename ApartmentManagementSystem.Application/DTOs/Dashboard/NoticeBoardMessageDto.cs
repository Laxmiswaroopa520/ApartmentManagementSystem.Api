namespace ApartmentManagementSystem.Application.DTOs.Dashboard
{
    public class NoticeBoardMessageDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty; // Low, Medium, High, Urgent
        public string Category { get; set; } = string.Empty; // Complaint, Announcement, Request
        public string PostedBy { get; set; } = string.Empty;
        public string FlatNumber { get; set; } = string.Empty;
        public DateTime PostedAt { get; set; }
        public bool IsResolved { get; set; }
    }
}