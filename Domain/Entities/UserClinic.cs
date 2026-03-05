using Domain.Enums;
using System;

namespace Domain.Entities
{
    public class UserClinic
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid ClinicId { get; set; }
        public Clinic Clinic { get; set; } = null!;

        public ClinicUserRole Role { get; set; } = ClinicUserRole.Member;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
