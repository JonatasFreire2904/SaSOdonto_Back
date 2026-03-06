using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Appointment;

namespace Api.Services
{
    public class AppointmentService
    {
        private readonly AppDbContext _db;

        public AppointmentService(AppDbContext db)
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
        /// Lista todos os atendimentos de uma clínica
        /// </summary>
        public async Task<List<AppointmentResponse>?> GetByClinicAsync(Guid userId, Guid clinicId)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            return await _db.Appointments
                .Where(a => a.ClinicId == clinicId)
                .OrderByDescending(a => a.ScheduledAt)
                .Select(a => new AppointmentResponse
                {
                    Id = a.Id,
                    Procedure = a.Procedure,
                    Description = a.Description,
                    Notes = a.Notes,
                    ScheduledAt = a.ScheduledAt,
                    CompletedAt = a.CompletedAt,
                    Status = a.Status,
                    Price = a.Price,
                    DentistName = a.DentistName,
                    Tooth = a.Tooth,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.FullName,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();
        }

        /// <summary>
        /// Lista atendimentos de um paciente específico
        /// </summary>
        public async Task<List<AppointmentResponse>?> GetByPatientAsync(Guid userId, Guid clinicId, Guid patientId)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            return await _db.Appointments
                .Where(a => a.ClinicId == clinicId && a.PatientId == patientId)
                .OrderByDescending(a => a.ScheduledAt)
                .Select(a => new AppointmentResponse
                {
                    Id = a.Id,
                    Procedure = a.Procedure,
                    Description = a.Description,
                    Notes = a.Notes,
                    ScheduledAt = a.ScheduledAt,
                    CompletedAt = a.CompletedAt,
                    Status = a.Status,
                    Price = a.Price,
                    DentistName = a.DentistName,
                    Tooth = a.Tooth,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.FullName,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();
        }

        /// <summary>
        /// Cria um novo atendimento (agendamento)
        /// </summary>
        public async Task<AppointmentResponse?> CreateAsync(Guid userId, Guid clinicId, CreateAppointmentRequest request)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            var patient = await _db.Patients
                .FirstOrDefaultAsync(p => p.Id == request.PatientId && p.ClinicId == clinicId);

            if (patient == null)
                return null;

            var appointment = new Appointment
            {
                Procedure = request.Procedure,
                Description = request.Description,
                Notes = request.Notes,
                ScheduledAt = request.ScheduledAt,
                Price = request.Price,
                DentistName = request.DentistName,
                Tooth = request.Tooth,
                PatientId = request.PatientId,
                ClinicId = clinicId
            };

            _db.Appointments.Add(appointment);
            await _db.SaveChangesAsync();

            return new AppointmentResponse
            {
                Id = appointment.Id,
                Procedure = appointment.Procedure,
                Description = appointment.Description,
                Notes = appointment.Notes,
                ScheduledAt = appointment.ScheduledAt,
                CompletedAt = appointment.CompletedAt,
                Status = appointment.Status,
                Price = appointment.Price,
                DentistName = appointment.DentistName,
                Tooth = appointment.Tooth,
                PatientId = appointment.PatientId,
                PatientName = patient.FullName,
                CreatedAt = appointment.CreatedAt
            };
        }

        /// <summary>
        /// Conclui um atendimento e gera automaticamente uma entrada no prontuário
        /// </summary>
        public async Task<AppointmentResponse?> CompleteAsync(Guid userId, Guid clinicId, Guid appointmentId, CompleteAppointmentRequest request)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            var appointment = await _db.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.ClinicId == clinicId);

            if (appointment == null)
                return null;

            appointment.Status = AppointmentStatus.Completed;
            appointment.CompletedAt = DateTime.UtcNow;

            if (request.Notes != null)
                appointment.Notes = request.Notes;

            // Gera entrada automática no prontuário
            var record = new MedicalRecord
            {
                Type = MedicalRecordType.Appointment,
                Title = $"Atendimento: {appointment.Procedure}",
                Description = request.RecordDescription ?? appointment.Description ?? $"Procedimento realizado: {appointment.Procedure}",
                PatientId = appointment.PatientId,
                AppointmentId = appointment.Id
            };

            _db.MedicalRecords.Add(record);
            await _db.SaveChangesAsync();

            return new AppointmentResponse
            {
                Id = appointment.Id,
                Procedure = appointment.Procedure,
                Description = appointment.Description,
                Notes = appointment.Notes,
                ScheduledAt = appointment.ScheduledAt,
                CompletedAt = appointment.CompletedAt,
                Status = appointment.Status,
                Price = appointment.Price,
                DentistName = appointment.DentistName,
                Tooth = appointment.Tooth,
                PatientId = appointment.PatientId,
                PatientName = appointment.Patient.FullName,
                CreatedAt = appointment.CreatedAt
            };
        }

        /// <summary>
        /// Cancela um atendimento
        /// </summary>
        public async Task<AppointmentResponse?> CancelAsync(Guid userId, Guid clinicId, Guid appointmentId)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            var appointment = await _db.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.ClinicId == clinicId);

            if (appointment == null)
                return null;

            appointment.Status = AppointmentStatus.Cancelled;

            await _db.SaveChangesAsync();

            return new AppointmentResponse
            {
                Id = appointment.Id,
                Procedure = appointment.Procedure,
                Description = appointment.Description,
                Notes = appointment.Notes,
                ScheduledAt = appointment.ScheduledAt,
                CompletedAt = appointment.CompletedAt,
                Status = appointment.Status,
                Price = appointment.Price,
                DentistName = appointment.DentistName,
                Tooth = appointment.Tooth,
                PatientId = appointment.PatientId,
                PatientName = appointment.Patient.FullName,
                CreatedAt = appointment.CreatedAt
            };
        }
    }
}
