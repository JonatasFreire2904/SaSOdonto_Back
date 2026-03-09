using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Anamnesis
{
    public class SaveAnamnesisRequest
    {
        // Doenças e condições
        public bool HasDiabetes { get; set; }
        public bool HasHypertension { get; set; }
        public bool HasHeartDisease { get; set; }
        public bool HasBleedingDisorder { get; set; }
        public bool HasHepatis { get; set; }
        public bool HasHiv { get; set; }
        public bool HasAsthma { get; set; }
        public bool HasSeizures { get; set; }

        // Hábitos
        public bool IsSmoker { get; set; }
        public bool IsAlcoholUser { get; set; }
        public bool UsesDrugs { get; set; }
        public bool IsPregnant { get; set; }
        public bool IsBreastfeeding { get; set; }

        // Texto livre
        [MaxLength(1000)]
        public string? Allergies { get; set; }

        [MaxLength(1000)]
        public string? Medications { get; set; }

        [MaxLength(1000)]
        public string? PreviousSurgeries { get; set; }

        [MaxLength(1000)]
        public string? FamilyHistory { get; set; }

        [MaxLength(2000)]
        public string? Observations { get; set; }

        // Campos personalizados
        public List<CustomFieldDto>? CustomFields { get; set; }
    }

    public class CustomFieldDto
    {
        [Required]
        [MaxLength(500)]
        public string Question { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Answer { get; set; }
    }
}
