using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.MedicalRecord;

namespace Api.Services
{
    public class MedicalRecordService
    {
        private readonly AppDbContext _db;

        public MedicalRecordService(AppDbContext db)
        {
            _db = db;
        }

        private async Task<bool> UserHasAccessAsync(Guid userId, Guid clinicId)
        {
            return await _db.Clinics.AnyAsync(c =>
                c.Id == clinicId &&
                (c.OwnerId == userId || c.UserClinics.Any(uc => uc.UserId == userId)));
        }

        /// <summary>
        /// Retorna todo o prontuário (histórico) de um paciente
        /// </summary>
        public async Task<List<MedicalRecordResponse>?> GetByPatientAsync(Guid userId, Guid clinicId, Guid patientId)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            return await _db.MedicalRecords
                .Where(m => m.PatientId == patientId && m.Patient.ClinicId == clinicId)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new MedicalRecordResponse
                {
                    Id = m.Id,
                    Type = m.Type,
                    Title = m.Title,
                    Description = m.Description,
                    AppointmentId = m.AppointmentId,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();
        }

        /// <summary>
        /// Adiciona uma entrada manual ao prontuário (ex: alergia, observação)
        /// </summary>
        public async Task<MedicalRecordResponse?> CreateAsync(Guid userId, Guid clinicId, Guid patientId, CreateMedicalRecordRequest request)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            var patientExists = await _db.Patients.AnyAsync(p => p.Id == patientId && p.ClinicId == clinicId);
            if (!patientExists)
                return null;

            var record = new MedicalRecord
            {
                Type = request.Type,
                Title = request.Title,
                Description = request.Description,
                PatientId = patientId
            };

            _db.MedicalRecords.Add(record);
            await _db.SaveChangesAsync();

            return new MedicalRecordResponse
            {
                Id = record.Id,
                Type = record.Type,
                Title = record.Title,
                Description = record.Description,
                AppointmentId = record.AppointmentId,
                CreatedAt = record.CreatedAt
            };
        }

        /// <summary>
        /// Retorna apenas os alertas médicos (alergias e condições crônicas)
        /// </summary>
        public async Task<List<Shared.DTOs.MedicalRecord.MedicalAlertResponse>?> GetAlertsAsync(Guid userId, Guid clinicId, Guid patientId)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            return await _db.MedicalRecords
                .Where(m => m.PatientId == patientId
                    && m.Patient.ClinicId == clinicId
                    && (m.Type == Domain.Enums.MedicalRecordType.Allergy || m.Type == Domain.Enums.MedicalRecordType.ChronicCondition))
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new Shared.DTOs.MedicalRecord.MedicalAlertResponse
                {
                    Id = m.Id,
                    Type = m.Type,
                    Title = m.Title,
                    Description = m.Description,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();
        }

        /// <summary>
        /// Remove uma entrada do prontuário
        /// </summary>
        public async Task<bool?> DeleteAsync(Guid userId, Guid clinicId, Guid patientId, Guid recordId)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            var record = await _db.MedicalRecords
                .FirstOrDefaultAsync(m => m.Id == recordId && m.PatientId == patientId && m.Patient.ClinicId == clinicId);

            if (record == null)
                return false;

            _db.MedicalRecords.Remove(record);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
