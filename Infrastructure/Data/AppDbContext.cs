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
        }
    }
}
