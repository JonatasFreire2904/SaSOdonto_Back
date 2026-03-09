namespace Shared.DTOs.Anamnesis
{
    public class AnamnesisResponse
    {
        public Guid Id { get; set; }

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
        public string? Allergies { get; set; }
        public string? Medications { get; set; }
        public string? PreviousSurgeries { get; set; }
        public string? FamilyHistory { get; set; }
        public string? Observations { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Campos personalizados
        public List<CustomFieldResponse> CustomFields { get; set; } = new();
    }

    public class CustomFieldResponse
    {
        public Guid Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string? Answer { get; set; }
    }
}
