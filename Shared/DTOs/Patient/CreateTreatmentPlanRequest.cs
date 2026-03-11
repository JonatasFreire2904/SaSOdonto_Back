using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.TreatmentPlan
{
    public class CreateTreatmentPlanRequest
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Category { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
