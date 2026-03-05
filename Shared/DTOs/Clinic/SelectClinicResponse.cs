namespace Shared.DTOs.Clinic
{
    public class SelectClinicResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
