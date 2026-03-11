using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Clinic> Clinics => Set<Clinic>();
        public DbSet<UserClinic> UserClinics => Set<UserClinic>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<OdontogramEntry> OdontogramEntries => Set<OdontogramEntry>();
        public DbSet<PatientMedia> PatientMedias => Set<PatientMedia>();
        public DbSet<Anamnesis> Anamneses => Set<Anamnesis>();
        public DbSet<AnamnesisCustomField> AnamnesisCustomFields => Set<AnamnesisCustomField>();
        public DbSet<TreatmentPlan> TreatmentPlans => Set<TreatmentPlan>();
        public DbSet<Procedure> Procedures => Set<Procedure>();
        public DbSet<ToothProcedure> ToothProcedures => Set<ToothProcedure>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.Id);
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.Email).IsRequired().HasMaxLength(200);
                e.Property(u => u.UserName).IsRequired().HasMaxLength(100);
                e.Property(u => u.PasswordHash).IsRequired();
            });

            // Clinic
            modelBuilder.Entity<Clinic>(e =>
            {
                e.HasKey(c => c.Id);
                e.Property(c => c.Name).IsRequired().HasMaxLength(200);
                e.Property(c => c.Location).HasMaxLength(300);
                e.Property(c => c.ImageUrl).HasMaxLength(500);
                e.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);

                e.HasOne(c => c.Owner)
                 .WithMany()
                 .HasForeignKey(c => c.OwnerId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // UserClinic (many-to-many)
            modelBuilder.Entity<UserClinic>(e =>
            {
                e.HasKey(uc => new { uc.UserId, uc.ClinicId });

                e.Property(uc => uc.Role).HasConversion<string>().HasMaxLength(20);

                e.HasOne(uc => uc.User)
                 .WithMany(u => u.UserClinics)
                 .HasForeignKey(uc => uc.UserId);

                e.HasOne(uc => uc.Clinic)
                 .WithMany(c => c.UserClinics)
                 .HasForeignKey(uc => uc.ClinicId);
            });

            // Patient
            modelBuilder.Entity<Patient>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.FullName).IsRequired().HasMaxLength(200);
                e.Property(p => p.Cpf).HasMaxLength(14);
                e.Property(p => p.Phone).HasMaxLength(20);
                e.Property(p => p.Email).HasMaxLength(200);
                e.Property(p => p.Address).HasMaxLength(300);
                e.Property(p => p.Notes).HasMaxLength(500);
                e.Property(p => p.Gender).HasConversion<string>().HasMaxLength(10);

                e.HasOne(p => p.Clinic)
                 .WithMany()
                 .HasForeignKey(p => p.ClinicId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // MedicalRecord (Prontuário)
            modelBuilder.Entity<MedicalRecord>(e =>
            {
                e.HasKey(m => m.Id);
                e.Property(m => m.Title).IsRequired().HasMaxLength(200);
                e.Property(m => m.Description).IsRequired().HasMaxLength(2000);
                e.Property(m => m.Type).HasConversion<string>().HasMaxLength(20);

                e.HasOne(m => m.Patient)
                 .WithMany(p => p.MedicalRecords)
                 .HasForeignKey(m => m.PatientId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(m => m.Appointment)
                 .WithOne(a => a.MedicalRecord)
                 .HasForeignKey<MedicalRecord>(m => m.AppointmentId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // Appointment (Atendimento)
            modelBuilder.Entity<Appointment>(e =>
            {
                e.HasKey(a => a.Id);
                e.Property(a => a.Procedure).IsRequired().HasMaxLength(200);
                e.Property(a => a.Description).HasMaxLength(1000);
                e.Property(a => a.Notes).HasMaxLength(1000);
                e.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
                e.Property(a => a.Price).HasColumnType("decimal(10,2)");

                e.HasOne(a => a.Patient)
                 .WithMany(p => p.Appointments)
                 .HasForeignKey(a => a.PatientId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(a => a.Clinic)
                 .WithMany()
                 .HasForeignKey(a => a.ClinicId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.Property(a => a.DentistName).HasMaxLength(200);
                e.Property(a => a.Tooth).HasMaxLength(50);
            });

            // OdontogramEntry (Odontograma)
            modelBuilder.Entity<OdontogramEntry>(e =>
            {
                e.HasKey(o => o.Id);
                e.Property(o => o.Notes).HasMaxLength(500);
                e.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);

                e.HasOne(o => o.Patient)
                 .WithMany(p => p.OdontogramEntries)
                 .HasForeignKey(o => o.PatientId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(o => new { o.PatientId, o.ToothNumber }).IsUnique();
            });

            // PatientMedia (Mídias do paciente)
            modelBuilder.Entity<PatientMedia>(e =>
            {
                e.HasKey(m => m.Id);
                e.Property(m => m.Name).IsRequired().HasMaxLength(200);
                e.Property(m => m.Url).IsRequired().HasMaxLength(500);
                e.Property(m => m.ThumbnailUrl).HasMaxLength(500);
                e.Property(m => m.Type).HasConversion<string>().HasMaxLength(20);

                e.HasOne(m => m.Patient)
                 .WithMany(p => p.PatientMedias)
                 .HasForeignKey(m => m.PatientId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Anamnesis (Anamnese)
            modelBuilder.Entity<Anamnesis>(e =>
            {
                e.HasKey(a => a.Id);
                e.Property(a => a.Allergies).HasMaxLength(1000);
                e.Property(a => a.Medications).HasMaxLength(1000);
                e.Property(a => a.PreviousSurgeries).HasMaxLength(1000);
                e.Property(a => a.FamilyHistory).HasMaxLength(1000);
                e.Property(a => a.Observations).HasMaxLength(2000);

                e.HasOne(a => a.Patient)
                 .WithOne(p => p.Anamnesis)
                 .HasForeignKey<Anamnesis>(a => a.PatientId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(a => a.PatientId).IsUnique();
            });

            // AnamnesisCustomField (Campos personalizados da anamnese)
            modelBuilder.Entity<AnamnesisCustomField>(e =>
            {
                e.HasKey(f => f.Id);
                e.Property(f => f.Question).IsRequired().HasMaxLength(500);
                e.Property(f => f.Answer).HasMaxLength(2000);

                e.HasOne(f => f.Anamnesis)
                 .WithMany(a => a.CustomFields)
                 .HasForeignKey(f => f.AnamnesisId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // TreatmentPlan (Plano de Tratamento)
            modelBuilder.Entity<TreatmentPlan>(e =>
            {
                e.HasKey(t => t.Id);
                e.Property(t => t.Name).IsRequired().HasMaxLength(200);
                e.Property(t => t.Category).HasMaxLength(100);

                e.HasOne(t => t.Patient)
                 .WithMany(p => p.TreatmentPlans)
                 .HasForeignKey(t => t.PatientId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Procedure (Procedimentos)
            modelBuilder.Entity<Procedure>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Name).IsRequired().HasMaxLength(200);
                e.Property(p => p.Category).IsRequired().HasMaxLength(100);

                e.HasOne(p => p.Clinic)
                 .WithMany()
                 .HasForeignKey(p => p.ClinicId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ToothProcedure (Procedimentos realizados em dentes)
            modelBuilder.Entity<ToothProcedure>(e =>
            {
                e.HasKey(tp => tp.Id);
                e.Property(tp => tp.Faces).IsRequired().HasMaxLength(50);
                e.Property(tp => tp.Notes).HasMaxLength(500);

                e.HasOne(tp => tp.Patient)
                 .WithMany(p => p.ToothProcedures)
                 .HasForeignKey(tp => tp.PatientId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(tp => tp.Procedure)
                 .WithMany()
                 .HasForeignKey(tp => tp.ProcedureId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
