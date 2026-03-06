using Domain.Enums;

namespace Shared.DTOs.MedicalRecord
{
    public class MedicalRecordResponse
    {
        public Guid Id { get; set; }
        public MedicalRecordType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? AppointmentId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
