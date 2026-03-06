using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Shared.DTOs.Appointment
{
    public class CompleteAppointmentRequest
    {
        [MaxLength(1000)]
        public string? Notes { get; set; }

        [MaxLength(2000)]
        public string? RecordDescription { get; set; }
    }
}
