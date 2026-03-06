using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Patient;

namespace Api.Services
{
    public class PatientService
    {
        private readonly AppDbContext _db;

        public PatientService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Verifica se o usuário tem acesso à clínica (é dono ou membro)
        /// </summary>
        public async Task<bool> UserHasAccessToClinicAsync(Guid userId, Guid clinicId)
        {
            return await _db.Clinics.AnyAsync(c =>
                c.Id == clinicId &&
                (c.OwnerId == userId || c.UserClinics.Any(uc => uc.UserId == userId)));
        }

        public async Task<List<PatientResponse>> GetPatientsByClinicAsync(Guid userId, Guid clinicId, string? search = null)
        {
            if (!await UserHasAccessToClinicAsync(userId, clinicId))
                return null!;

            var query = _db.Patients.Where(p => p.ClinicId == clinicId);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.FullName.Contains(search) || (p.Cpf != null && p.Cpf.Contains(search)));

            return await query
                .OrderBy(p => p.FullName)
                .Select(p => new PatientResponse
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    Cpf = p.Cpf,
                    BirthDate = p.BirthDate,
                    Gender = p.Gender,
                    Phone = p.Phone,
                    Email = p.Email,
                    Address = p.Address,
                    Notes = p.Notes,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<PatientResponse?> GetPatientByIdAsync(Guid userId, Guid clinicId, Guid patientId)
        {
            if (!await UserHasAccessToClinicAsync(userId, clinicId))
                return null;

            return await _db.Patients
                .Where(p => p.Id == patientId && p.ClinicId == clinicId)
                .Select(p => new PatientResponse
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    Cpf = p.Cpf,
                    BirthDate = p.BirthDate,
                    Gender = p.Gender,
                    Phone = p.Phone,
                    Email = p.Email,
                    Address = p.Address,
                    Notes = p.Notes,
                    CreatedAt = p.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PatientResponse?> CreatePatientAsync(Guid userId, Guid clinicId, CreatePatientRequest request)
        {
            if (!await UserHasAccessToClinicAsync(userId, clinicId))
                return null;

            var patient = new Patient
            {
                FullName = request.FullName,
                Cpf = request.Cpf,
                BirthDate = request.BirthDate,
                Gender = request.Gender,
                Phone = request.Phone,
                Email = request.Email,
                Address = request.Address,
                Notes = request.Notes,
                ClinicId = clinicId
            };

            _db.Patients.Add(patient);
            await _db.SaveChangesAsync();

            return new PatientResponse
            {
                Id = patient.Id,
                FullName = patient.FullName,
                Cpf = patient.Cpf,
                BirthDate = patient.BirthDate,
                Gender = patient.Gender,
                Phone = patient.Phone,
                Email = patient.Email,
                Address = patient.Address,
                Notes = patient.Notes,
                CreatedAt = patient.CreatedAt
            };
        }

        public async Task<PatientResponse?> UpdatePatientAsync(Guid userId, Guid clinicId, Guid patientId, UpdatePatientRequest request)
        {
            if (!await UserHasAccessToClinicAsync(userId, clinicId))
                return null;

            var patient = await _db.Patients
                .FirstOrDefaultAsync(p => p.Id == patientId && p.ClinicId == clinicId);

            if (patient == null)
                return null;

            patient.FullName = request.FullName;
            patient.Cpf = request.Cpf;
            patient.BirthDate = request.BirthDate;
            patient.Gender = request.Gender;
            patient.Phone = request.Phone;
            patient.Email = request.Email;
            patient.Address = request.Address;
            patient.Notes = request.Notes;

            await _db.SaveChangesAsync();

            return new PatientResponse
            {
                Id = patient.Id,
                FullName = patient.FullName,
                Cpf = patient.Cpf,
                BirthDate = patient.BirthDate,
                Gender = patient.Gender,
                Phone = patient.Phone,
                Email = patient.Email,
                Address = patient.Address,
                Notes = patient.Notes,
                CreatedAt = patient.CreatedAt
            };
        }

        public async Task<bool?> DeletePatientAsync(Guid userId, Guid clinicId, Guid patientId)
        {
            if (!await UserHasAccessToClinicAsync(userId, clinicId))
                return null;

            var patient = await _db.Patients
                .FirstOrDefaultAsync(p => p.Id == patientId && p.ClinicId == clinicId);

            if (patient == null)
                return false;

            _db.Patients.Remove(patient);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
