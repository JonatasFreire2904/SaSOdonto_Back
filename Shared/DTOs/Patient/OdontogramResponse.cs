using Domain.Enums;

namespace Shared.DTOs.Odontogram
{
    public class ToothResponse
    {
        public int Number { get; set; }
        public ToothStatus Status { get; set; }
        public string? Notes { get; set; }
    }

    public class OdontogramResponse
    {
        public List<ToothResponse> UpperTeeth { get; set; } = new();
        public List<ToothResponse> LowerTeeth { get; set; } = new();
    }
}
