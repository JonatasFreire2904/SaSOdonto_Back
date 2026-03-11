using Domain.Enums;

namespace Domain.Entities
{
    public class PatientMedia
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public MediaType Type { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
    }
}
