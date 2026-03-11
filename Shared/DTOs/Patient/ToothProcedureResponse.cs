namespace Shared.DTOs.Odontogram
{
    public class ToothProcedureResponse
    {
        public Guid Id { get; set; }
        public int ToothNumber { get; set; }
        public string ProcedureName { get; set; } = string.Empty;
        public string ProcedureCategory { get; set; } = string.Empty;
        public string Faces { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
