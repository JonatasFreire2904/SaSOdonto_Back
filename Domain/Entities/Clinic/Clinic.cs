using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Clinic
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public ClinicStatus Status { get; set; } = ClinicStatus.Active;
        public Guid OwnerId { get; set; }
        public User Owner { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserClinic> UserClinics { get; set; } = new List<UserClinic>();
    }
}
