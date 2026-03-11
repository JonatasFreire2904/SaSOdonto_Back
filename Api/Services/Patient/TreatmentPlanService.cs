using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.TreatmentPlan;

namespace Api.Services
{
    public class TreatmentPlanService
    {
        private readonly AppDbContext _db;

        public TreatmentPlanService(AppDbContext db)
        {
            _db = db;
        }

        private async Task<bool> UserHasAccessToClinicAsync(Guid userId, Guid clinicId)
        {
            return await _db.Clinics.AnyAsync(c =>
                c.Id == clinicId &&
                (c.OwnerId == userId || c.UserClinics.Any(uc => uc.UserId == userId)));
        }

        public async Task<List<TreatmentPlanResponse>?> GetByPatientAsync(Guid userId, Guid clinicId, Guid patientId)
        {
            if (!await UserHasAccessToClinicAsync(userId, clinicId))
                return null;

            var patientExists = await _db.Patients.AnyAsync(p => p.Id == patientId && p.ClinicId == clinicId);
            if (!patientExists)
                return null;

            return await _db.TreatmentPlans
                .Where(t => t.PatientId == patientId)
                .OrderByDescending(t => t.Date)
                .Select(t => new TreatmentPlanResponse
                {
                    Id = t.Id,
                    Name = t.Name,
                    Category = t.Category,
                    Date = t.Date,
                    IsCompleted = t.IsCompleted,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<TreatmentPlanResponse?> CreateAsync(Guid userId, Guid clinicId, Guid patientId, CreateTreatmentPlanRequest request)
        {
            if (!await UserHasAccessToClinicAsync(userId, clinicId))
                return null;

            var patientExists = await _db.Patients.AnyAsync(p => p.Id == patientId && p.ClinicId == clinicId);
            if (!patientExists)
                return null;

            var plan = new TreatmentPlan
            {
                Name = request.Name,
                Category = request.Category,
                Date = request.Date,
                PatientId = patientId
            };

            _db.TreatmentPlans.Add(plan);
            await _db.SaveChangesAsync();

            return new TreatmentPlanResponse
            {
                Id = plan.Id,
                Name = plan.Name,
                Category = plan.Category,
                Date = plan.Date,
                IsCompleted = plan.IsCompleted,
                CreatedAt = plan.CreatedAt
            };
        }

        public async Task<TreatmentPlanResponse?> UpdateAsync(Guid userId, Guid clinicId, Guid patientId, Guid planId, UpdateTreatmentPlanRequest request)
        {
            if (!await UserHasAccessToClinicAsync(userId, clinicId))
                return null;

            var plan = await _db.TreatmentPlans
                .Include(t => t.Patient)
                .FirstOrDefaultAsync(t => t.Id == planId && t.PatientId == patientId && t.Patient.ClinicId == clinicId);

            if (plan == null)
                return null;

            plan.Name = request.Name;
            plan.Category = request.Category;
            plan.Date = request.Date;
            plan.IsCompleted = request.IsCompleted;

            await _db.SaveChangesAsync();

            return new TreatmentPlanResponse
            {
                Id = plan.Id,
                Name = plan.Name,
                Category = plan.Category,
                Date = plan.Date,
                IsCompleted = plan.IsCompleted,
                CreatedAt = plan.CreatedAt
            };
        }

        public async Task<bool?> DeleteAsync(Guid userId, Guid clinicId, Guid patientId, Guid planId)
        {
            if (!await UserHasAccessToClinicAsync(userId, clinicId))
                return null;

            var plan = await _db.TreatmentPlans
                .Include(t => t.Patient)
                .FirstOrDefaultAsync(t => t.Id == planId && t.PatientId == patientId && t.Patient.ClinicId == clinicId);

            if (plan == null)
                return false;

            _db.TreatmentPlans.Remove(plan);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool?> DeleteAllByPatientAsync(Guid userId, Guid clinicId, Guid patientId)
        {
            if (!await UserHasAccessToClinicAsync(userId, clinicId))
                return null;

            var patientExists = await _db.Patients.AnyAsync(p => p.Id == patientId && p.ClinicId == clinicId);
            if (!patientExists)
                return false;

            var plans = await _db.TreatmentPlans
                .Where(t => t.PatientId == patientId)
                .ToListAsync();

            if (plans.Count == 0)
                return false;

            _db.TreatmentPlans.RemoveRange(plans);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
