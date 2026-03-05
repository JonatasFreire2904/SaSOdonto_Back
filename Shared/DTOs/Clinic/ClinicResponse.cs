namespace Shared.DTOs.Clinic
{
    public class ClinicResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TeamCount { get; set; }
    }
}
