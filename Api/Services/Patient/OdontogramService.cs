using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Odontogram;

namespace Api.Services
{
    public class OdontogramService
    {
        private readonly AppDbContext _db;

        // Dentes superiores (notação FDI): 18-11, 21-28
        // Dentes inferiores (notação FDI): 48-41, 31-38
        private static readonly int[] UpperTeeth = { 18, 17, 16, 15, 14, 13, 12, 11, 21, 22, 23, 24, 25, 26, 27, 28 };
        private static readonly int[] LowerTeeth = { 48, 47, 46, 45, 44, 43, 42, 41, 31, 32, 33, 34, 35, 36, 37, 38 };

        public OdontogramService(AppDbContext db)
        {
            _db = db;
        }

        private async Task<bool> UserHasAccessAsync(Guid userId, Guid clinicId)
        {
            return await _db.Clinics.AnyAsync(c =>
                c.Id == clinicId &&
                (c.OwnerId == userId || c.UserClinics.Any(uc => uc.UserId == userId)));
        }

        public async Task<OdontogramResponse?> GetByPatientAsync(Guid userId, Guid clinicId, Guid patientId)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            var patientExists = await _db.Patients.AnyAsync(p => p.Id == patientId && p.ClinicId == clinicId);
            if (!patientExists)
                return null;

            var entries = await _db.OdontogramEntries
                .Where(o => o.PatientId == patientId)
                .ToListAsync();

            // Se não tem odontograma ainda, gera os 32 dentes com status normal
            if (entries.Count == 0)
            {
                var allTeeth = UpperTeeth.Concat(LowerTeeth);
                foreach (var tooth in allTeeth)
                {
                    entries.Add(new OdontogramEntry
                    {
                        PatientId = patientId,
                        ToothNumber = tooth
                    });
                }
                _db.OdontogramEntries.AddRange(entries);
                await _db.SaveChangesAsync();
            }

            var lookup = entries.ToDictionary(e => e.ToothNumber);

            return new OdontogramResponse
            {
                UpperTeeth = UpperTeeth.Select(n => new ToothResponse
                {
                    Number = n,
                    Status = lookup.ContainsKey(n) ? lookup[n].Status : ToothStatus.Normal,
                    Notes = lookup.ContainsKey(n) ? lookup[n].Notes : null
                }).ToList(),
                LowerTeeth = LowerTeeth.Select(n => new ToothResponse
                {
                    Number = n,
                    Status = lookup.ContainsKey(n) ? lookup[n].Status : ToothStatus.Normal,
                    Notes = lookup.ContainsKey(n) ? lookup[n].Notes : null
                }).ToList()
            };
        }

        public async Task<ToothResponse?> UpdateToothAsync(Guid userId, Guid clinicId, Guid patientId, int toothNumber, UpdateToothRequest request)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            var patientExists = await _db.Patients.AnyAsync(p => p.Id == patientId && p.ClinicId == clinicId);
            if (!patientExists)
                return null;

            var entry = await _db.OdontogramEntries
                .FirstOrDefaultAsync(o => o.PatientId == patientId && o.ToothNumber == toothNumber);

            if (entry == null)
            {
                entry = new OdontogramEntry
                {
                    PatientId = patientId,
                    ToothNumber = toothNumber,
                    Status = request.Status,
                    Notes = request.Notes
                };
                _db.OdontogramEntries.Add(entry);
            }
            else
            {
                entry.Status = request.Status;
                entry.Notes = request.Notes;
                entry.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            return new ToothResponse
            {
                Number = entry.ToothNumber,
                Status = entry.Status,
                Notes = entry.Notes
            };
        }

        public async Task<ToothProcedureResponse?> AddToothProcedureAsync(Guid userId, Guid clinicId, Guid patientId, CreateToothProcedureRequest request)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            var patientExists = await _db.Patients.AnyAsync(p => p.Id == patientId && p.ClinicId == clinicId);
            if (!patientExists)
                return null;

            var procedure = await _db.Procedures.FirstOrDefaultAsync(p => p.Id == request.ProcedureId && p.ClinicId == clinicId);
            if (procedure == null)
                return null;

            var toothProcedure = new ToothProcedure
            {
                ToothNumber = request.ToothNumber,
                ProcedureId = request.ProcedureId,
                Faces = request.Faces,
                Notes = request.Notes,
                PatientId = patientId
            };

            _db.ToothProcedures.Add(toothProcedure);
            await _db.SaveChangesAsync();

            return new ToothProcedureResponse
            {
                Id = toothProcedure.Id,
                ToothNumber = toothProcedure.ToothNumber,
                ProcedureName = procedure.Name,
                ProcedureCategory = procedure.Category,
                Faces = toothProcedure.Faces,
                Notes = toothProcedure.Notes,
                CreatedAt = toothProcedure.CreatedAt
            };
        }

        public async Task<List<ToothProcedureResponse>?> GetToothProceduresAsync(Guid userId, Guid clinicId, Guid patientId)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            var patientExists = await _db.Patients.AnyAsync(p => p.Id == patientId && p.ClinicId == clinicId);
            if (!patientExists)
                return null;

            return await _db.ToothProcedures
                .Where(tp => tp.PatientId == patientId)
                .Include(tp => tp.Procedure)
                .OrderByDescending(tp => tp.CreatedAt)
                .Select(tp => new ToothProcedureResponse
                {
                    Id = tp.Id,
                    ToothNumber = tp.ToothNumber,
                    ProcedureName = tp.Procedure.Name,
                    ProcedureCategory = tp.Procedure.Category,
                    Faces = tp.Faces,
                    Notes = tp.Notes,
                    CreatedAt = tp.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<bool?> DeleteToothProcedureAsync(Guid userId, Guid clinicId, Guid patientId, Guid toothProcedureId)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            var tp = await _db.ToothProcedures
                .Include(t => t.Patient)
                .FirstOrDefaultAsync(t => t.Id == toothProcedureId && t.PatientId == patientId && t.Patient.ClinicId == clinicId);

            if (tp == null)
                return false;

            _db.ToothProcedures.Remove(tp);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
