namespace Domain.Entities
{
    public class Anamnesis
    {
        public Guid Id { get; set; } = Guid.NewGuid();

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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
    }
}
