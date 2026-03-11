namespace Domain.Entities
{
    public class AnamnesisCustomField
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Question { get; set; } = string.Empty;
        public string? Answer { get; set; }

        public Guid AnamnesisId { get; set; }
        public Anamnesis Anamnesis { get; set; } = null!;
    }
}
