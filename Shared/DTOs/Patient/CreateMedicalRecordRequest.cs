using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Shared.DTOs.MedicalRecord
{
    public class CreateMedicalRecordRequest
    {
        [Required]
        public MedicalRecordType Type { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(2000)]
        public string Description { get; set; } = string.Empty;
    }
}
