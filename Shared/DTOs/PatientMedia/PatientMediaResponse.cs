using Domain.Enums;

namespace Shared.DTOs.PatientMedia
{
    public class PatientMediaResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public MediaType Type { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
