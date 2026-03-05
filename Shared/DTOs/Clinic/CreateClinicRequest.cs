using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Clinic
{
    public class CreateClinicRequest
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }
    }
}
