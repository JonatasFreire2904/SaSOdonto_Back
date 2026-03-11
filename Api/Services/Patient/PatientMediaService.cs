using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.PatientMedia;

namespace Api.Services
{
    public class PatientMediaService
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public PatientMediaService(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        private async Task<bool> UserHasAccessAsync(Guid userId, Guid clinicId)
        {
            return await _db.Clinics.AnyAsync(c =>
                c.Id == clinicId &&
                (c.OwnerId == userId || c.UserClinics.Any(uc => uc.UserId == userId)));
        }

        private static MediaType DetectMediaType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" or ".bmp" => MediaType.Image,
                ".pdf" => MediaType.Pdf,
                _ => MediaType.Document
            };
        }

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp",
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt"
        };

        public async Task<List<PatientMediaResponse>?> GetByPatientAsync(Guid userId, Guid clinicId, Guid patientId)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            return await _db.PatientMedias
                .Where(m => m.PatientId == patientId && m.Patient.ClinicId == clinicId)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new PatientMediaResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Type = m.Type,
                    Url = m.Url,
                    ThumbnailUrl = m.ThumbnailUrl,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<PatientMediaResponse?> UploadAsync(Guid userId, Guid clinicId, Guid patientId, IFormFile file, string? name)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            var patientExists = await _db.Patients.AnyAsync(p => p.Id == patientId && p.ClinicId == clinicId);
            if (!patientExists)
                return null;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return null;

            var uploadsDir = Path.Combine(_env.ContentRootPath, "uploads", clinicId.ToString(), patientId.ToString());
            Directory.CreateDirectory(uploadsDir);

            var fileId = Guid.NewGuid();
            var safeFileName = $"{fileId}{ext}";
            var filePath = Path.Combine(uploadsDir, safeFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var mediaType = DetectMediaType(file.FileName);
            var relativeUrl = $"/uploads/{clinicId}/{patientId}/{safeFileName}";

            var media = new PatientMedia
            {
                Name = name ?? Path.GetFileNameWithoutExtension(file.FileName),
                Type = mediaType,
                Url = relativeUrl,
                ThumbnailUrl = mediaType == MediaType.Image ? relativeUrl : null,
                PatientId = patientId
            };

            _db.PatientMedias.Add(media);
            await _db.SaveChangesAsync();

            return new PatientMediaResponse
            {
                Id = media.Id,
                Name = media.Name,
                Type = media.Type,
                Url = media.Url,
                ThumbnailUrl = media.ThumbnailUrl,
                CreatedAt = media.CreatedAt
            };
        }

        public async Task<bool?> DeleteAsync(Guid userId, Guid clinicId, Guid patientId, Guid mediaId)
        {
            if (!await UserHasAccessAsync(userId, clinicId))
                return null;

            var media = await _db.PatientMedias
                .FirstOrDefaultAsync(m => m.Id == mediaId && m.PatientId == patientId && m.Patient.ClinicId == clinicId);

            if (media == null)
                return false;

            // Remove arquivo físico
            var filePath = Path.Combine(_env.ContentRootPath, media.Url.TrimStart('/'));
            if (File.Exists(filePath))
                File.Delete(filePath);

            _db.PatientMedias.Remove(media);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
