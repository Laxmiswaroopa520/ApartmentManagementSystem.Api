namespace ApartmentManagementSystem.Application.DTOs.Dashboard
{
    public class QuickActionDto
    {
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public bool RequiresPermission { get; set; }
    }

}
