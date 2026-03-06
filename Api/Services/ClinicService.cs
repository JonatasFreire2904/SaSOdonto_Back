using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Clinic;

namespace Api.Services
{
    public class ClinicService
    {
        private readonly AppDbContext _db;

        public ClinicService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ClinicResponse>> GetClinicsByUserAsync(Guid userId)
        {
            var clinics = await _db.Clinics
                .Where(c => c.OwnerId == userId || c.UserClinics.Any(uc => uc.UserId == userId))
                .Select(c => new ClinicResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Location = c.Location,
                    ImageUrl = c.ImageUrl,
                    Status = c.Status,
                    TeamCount = c.UserClinics.Count
                })
                .ToListAsync();

            return clinics;
        }

        public async Task<ClinicResponse> CreateClinicAsync(Guid userId, CreateClinicRequest request)
        {
            var clinic = new Clinic
            {
                Name = request.Name,
                Location = request.Location,
                ImageUrl = request.ImageUrl,
                OwnerId = userId
            };

            _db.Clinics.Add(clinic);

            var userClinic = new UserClinic
            {
                UserId = userId,
                ClinicId = clinic.Id,
                Role = ClinicUserRole.Owner
            };

            _db.UserClinics.Add(userClinic);

            await _db.SaveChangesAsync();

            return new ClinicResponse
            {
                Id = clinic.Id,
                Name = clinic.Name,
                Location = clinic.Location,
                ImageUrl = clinic.ImageUrl,
                Status = clinic.Status,
                TeamCount = 1
            };
        }

        public async Task<SelectClinicResponse?> SelectClinicAsync(Guid userId, Guid clinicId)
        {
            var clinic = await _db.Clinics
                .Where(c => c.Id == clinicId &&
                       (c.OwnerId == userId || c.UserClinics.Any(uc => uc.UserId == userId)))
                .FirstOrDefaultAsync();

            if (clinic == null)
                return null;

            return new SelectClinicResponse
            {
                Id = clinic.Id,
                Name = clinic.Name,
                Location = clinic.Location,
                ImageUrl = clinic.ImageUrl,
                Status = clinic.Status
            };
        }
    }
}
