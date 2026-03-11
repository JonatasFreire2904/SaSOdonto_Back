namespace Shared.DTOs.Procedure
{
    public class ProcedureResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}
