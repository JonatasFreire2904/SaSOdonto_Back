using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Anamnesis;

namespace Api.Services
{
    public class AnamnesisService
    {
        private readonly AppDbContext _db;

        public AnamnesisService(AppDbContext db)
        {
            _db = db;
        }

        private async Task<bool> UserHasAccessAsync(Guid userId, Guid clinicId)
        {
            return await _db.Clinics.AnyAsync(c =>
                c.Id == clinicId &&
                (c.OwnerId == userId || c.UserClinics.Any(uc => uc.UserId == userId)));
        }

        public async Task<AnamnesisResponse?> GetByPatientAsync(Guid userId, Guid clinicId, Guid patientId)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            var anamnesis = await _db.Anamneses
                .Include(a => a.CustomFields)
                .FirstOrDefaultAsync(a => a.PatientId == patientId && a.Patient.ClinicId == clinicId);

            if (anamnesis == null)
                return new AnamnesisResponse(); // retorna vazio (formulário em branco)

            return MapToResponse(anamnesis);
        }

        public async Task<AnamnesisResponse?> SaveAsync(Guid userId, Guid clinicId, Guid patientId, SaveAnamnesisRequest request)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            var patientExists = await _db.Patients.AnyAsync(p => p.Id == patientId && p.ClinicId == clinicId);
            if (!patientExists)
                return null;

            var anamnesis = await _db.Anamneses
                .Include(a => a.CustomFields)
                .FirstOrDefaultAsync(a => a.PatientId == patientId);

            if (anamnesis == null)
            {
                anamnesis = new Anamnesis { PatientId = patientId };
                _db.Anamneses.Add(anamnesis);
            }

            anamnesis.HasDiabetes = request.HasDiabetes;
            anamnesis.HasHypertension = request.HasHypertension;
            anamnesis.HasHeartDisease = request.HasHeartDisease;
            anamnesis.HasBleedingDisorder = request.HasBleedingDisorder;
            anamnesis.HasHepatis = request.HasHepatis;
            anamnesis.HasHiv = request.HasHiv;
            anamnesis.HasAsthma = request.HasAsthma;
            anamnesis.HasSeizures = request.HasSeizures;
            anamnesis.IsSmoker = request.IsSmoker;
            anamnesis.IsAlcoholUser = request.IsAlcoholUser;
            anamnesis.UsesDrugs = request.UsesDrugs;
            anamnesis.IsPregnant = request.IsPregnant;
            anamnesis.IsBreastfeeding = request.IsBreastfeeding;
            anamnesis.Allergies = request.Allergies;
            anamnesis.Medications = request.Medications;
            anamnesis.PreviousSurgeries = request.PreviousSurgeries;
            anamnesis.FamilyHistory = request.FamilyHistory;
            anamnesis.Observations = request.Observations;
            anamnesis.UpdatedAt = DateTime.UtcNow;

            // Campos personalizados: remove antigos e adiciona novos
            if (anamnesis.CustomFields.Any())
                _db.AnamnesisCustomFields.RemoveRange(anamnesis.CustomFields);

            if (request.CustomFields != null)
            {
                foreach (var field in request.CustomFields)
                {
                    anamnesis.CustomFields.Add(new AnamnesisCustomField
                    {
                        Question = field.Question,
                        Answer = field.Answer
                    });
                }
            }

            await _db.SaveChangesAsync();

            return MapToResponse(anamnesis);
        }

        private static AnamnesisResponse MapToResponse(Anamnesis a) => new()
        {
            Id = a.Id,
            HasDiabetes = a.HasDiabetes,
            HasHypertension = a.HasHypertension,
            HasHeartDisease = a.HasHeartDisease,
            HasBleedingDisorder = a.HasBleedingDisorder,
            HasHepatis = a.HasHepatis,
            HasHiv = a.HasHiv,
            HasAsthma = a.HasAsthma,
            HasSeizures = a.HasSeizures,
            IsSmoker = a.IsSmoker,
            IsAlcoholUser = a.IsAlcoholUser,
            UsesDrugs = a.UsesDrugs,
            IsPregnant = a.IsPregnant,
            IsBreastfeeding = a.IsBreastfeeding,
            Allergies = a.Allergies,
            Medications = a.Medications,
            PreviousSurgeries = a.PreviousSurgeries,
            FamilyHistory = a.FamilyHistory,
            Observations = a.Observations,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt,
            CustomFields = a.CustomFields.Select(f => new CustomFieldResponse
            {
                Id = f.Id,
                Question = f.Question,
                Answer = f.Answer
            }).ToList()
        };
    }
}
