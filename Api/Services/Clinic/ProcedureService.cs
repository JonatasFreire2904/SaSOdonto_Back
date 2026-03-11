using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Procedure;

namespace Api.Services
{
    public class ProcedureService
    {
        private readonly AppDbContext _db;

        public ProcedureService(AppDbContext db)
        {
            _db = db;
        }

        private async Task<bool> UserHasAccessToClinicAsync(Guid userId, Guid clinicId)
        {
            return await _db.Clinics.AnyAsync(c =>
                c.Id == clinicId &&
                (c.OwnerId == userId || c.UserClinics.Any(uc => uc.UserId == userId)));
        }

        public async Task SeedDefaultProceduresAsync(Guid clinicId)
        {
            var hasAny = await _db.Procedures.AnyAsync(p => p.ClinicId == clinicId);
            if (hasAny) return;

            var defaults = new List<(string Category, string Name)>
            {
                // Diagnóstico e Prevenção
                ("Diagnóstico e Prevenção", "Avaliação / Consulta Inicial"),
                ("Diagnóstico e Prevenção", "Profilaxia (Limpeza)"),
                ("Diagnóstico e Prevenção", "Raspagem Supragengival"),
                ("Diagnóstico e Prevenção", "Aplicação de Flúor"),
                ("Diagnóstico e Prevenção", "Radiografia Periapical/Interproximal"),

                // Dentística Restauradora
                ("Dentística Restauradora", "Restauração de Resina - Classe I"),
                ("Dentística Restauradora", "Restauração de Resina - Classe II"),
                ("Dentística Restauradora", "Restauração de Resina - Classe III"),
                ("Dentística Restauradora", "Restauração de Resina - Classe IV"),
                ("Dentística Restauradora", "Restauração de Resina - Classe V"),
                ("Dentística Restauradora", "Ajuste Oclusal"),
                ("Dentística Restauradora", "Faceta Direta em Resina"),
                ("Dentística Restauradora", "Núcleo de Preenchimento"),

                // Cirurgia Oral Menor
                ("Cirurgia Oral Menor", "Exodontia Simples"),
                ("Cirurgia Oral Menor", "Exodontia de Terceiro Molar (Siso)"),
                ("Cirurgia Oral Menor", "Ulectomia"),
                ("Cirurgia Oral Menor", "Frenectomia"),

                // Endodontia
                ("Endodontia", "Tratamento de Canal - Unirradicular"),
                ("Endodontia", "Tratamento de Canal - Birradicular"),
                ("Endodontia", "Tratamento de Canal - Multirradicular"),
                ("Endodontia", "Urgência / Pulpotomia"),
                ("Endodontia", "Retratamento de Canal"),

                // Periodontia
                ("Periodontia", "Raspagem Subgengival"),
                ("Periodontia", "Gengivoplastia / Gengivectomia"),

                // Prótese
                ("Prótese", "Coroa Total (Porcelana/Zircônia)"),
                ("Prótese", "Prótese Total (Dentadura)"),
                ("Prótese", "Prótese Parcial Removível (PPR)"),
                ("Prótese", "Provisório"),

                // Ortodontia e Estética
                ("Ortodontia e Estética", "Instalação de Aparelho Fixo"),
                ("Ortodontia e Estética", "Instalação de Aparelho Autoligado"),
                ("Ortodontia e Estética", "Instalação de Alinhadores"),
                ("Ortodontia e Estética", "Manutenção Ortodôntica"),
                ("Ortodontia e Estética", "Clareamento de Consultório"),
                ("Ortodontia e Estética", "Clareamento Caseiro"),
                ("Ortodontia e Estética", "Aplicação de Toxina Botulínica (Botox)"),
            };

            var procedures = defaults.Select(d => new Procedure
            {
                Name = d.Name,
                Category = d.Category,
                IsDefault = true,
                ClinicId = clinicId
            }).ToList();

            _db.Procedures.AddRange(procedures);
            await _db.SaveChangesAsync();
        }

        public async Task<List<ProcedureResponse>?> GetByClinicAsync(Guid userId, Guid clinicId)
        {
            if (!await UserHasAccessToClinicAsync(userId, clinicId))
                return null;

            // Seed padrão na primeira consulta
            await SeedDefaultProceduresAsync(clinicId);

            return await _db.Procedures
                .Where(p => p.ClinicId == clinicId)
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .Select(p => new ProcedureResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Category = p.Category,
                    IsDefault = p.IsDefault
                })
                .ToListAsync();
        }

        public async Task<ProcedureResponse?> CreateAsync(Guid userId, Guid clinicId, CreateProcedureRequest request)
        {
            if (!await UserHasAccessToClinicAsync(userId, clinicId))
                return null;

            var procedure = new Procedure
            {
                Name = request.Name,
                Category = request.Category,
                IsDefault = false,
                ClinicId = clinicId
            };

            _db.Procedures.Add(procedure);
            await _db.SaveChangesAsync();

            return new ProcedureResponse
            {
                Id = procedure.Id,
                Name = procedure.Name,
                Category = procedure.Category,
                IsDefault = procedure.IsDefault
            };
        }

        public async Task<bool?> DeleteAsync(Guid userId, Guid clinicId, Guid procedureId)
        {
            if (!await UserHasAccessToClinicAsync(userId, clinicId))
                return null;

            var procedure = await _db.Procedures
                .FirstOrDefaultAsync(p => p.Id == procedureId && p.ClinicId == clinicId);

            if (procedure == null)
                return false;

            _db.Procedures.Remove(procedure);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
