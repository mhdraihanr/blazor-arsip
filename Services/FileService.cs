using Microsoft.EntityFrameworkCore;
using blazor_arsip.Data;
using blazor_arsip.Models;
using blazor_arsip.Components.Pages.Logs;
using System.Security.Cryptography;
using System.Text;

namespace blazor_arsip.Services;

public interface IFileService
{
    Task<FileRecord?> GetFileByIdAsync(int id);
    Task<IEnumerable<FileRecord>> GetAllFilesAsync();
    Task<IEnumerable<FileRecord>> SearchFilesAsync(string searchTerm, string? category = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<FileRecord> UploadFileAsync(IFormFile file, string uploadedBy, string? description = null, string? tags = null, string category = "Documents");
    Task<FileRecord> UpdateFileAsync(FileRecord fileRecord);
    Task<FileRecord> UpdateLastAccessedTimeAsync(FileRecord fileRecord);
    Task<bool> DeleteFileAsync(int id, string? performedBy = null, string? ipAddress = null, string? userAgent = null);
    Task<string> GetFilePathAsync(int id);
    Task<bool> FileExistsAsync(int id);
    Task<IEnumerable<FileActivity>> GetFileActivitiesAsync(int fileId);
    Task LogActivityAsync(int fileId, string activityType, string performedBy, string? description = null, string? ipAddress = null, string? userAgent = null);
    Task<IEnumerable<FileCategory>> GetCategoriesAsync();
    Task<Dictionary<string, object>> GetDashboardStatsAsync();
    Task<ActivitiesResult> GetAllActivitiesWithUsersAsync(string? searchTerm = null, string? activityType = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 50);
}

public class FileService : IFileService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileService> _logger;
    private readonly string _uploadsPath;

    public FileService(ApplicationDbContext context, IWebHostEnvironment environment, ILogger<FileService> logger)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
        _uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
        
        // Ensure uploads directory exists
        if (!Directory.Exists(_uploadsPath))
        {
            Directory.CreateDirectory(_uploadsPath);
        }
    }

    public async Task<FileRecord?> GetFileByIdAsync(int id)
    {
        return await _context.FileRecords
            .Include(f => f.Versions)
            .Include(f => f.Activities)
            .FirstOrDefaultAsync(f => f.Id == id && f.IsActive);
    }

    public async Task<IEnumerable<FileRecord>> GetAllFilesAsync()
    {
        return await _context.FileRecords
            .Where(f => f.IsActive)
            .OrderByDescending(f => f.UploadedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<FileRecord>> SearchFilesAsync(string searchTerm, string? category = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.FileRecords.Where(f => f.IsActive);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(f => 
                f.OriginalFileName.Contains(searchTerm) ||
                f.Description!.Contains(searchTerm) ||
                f.Tags!.Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(f => f.Category == category);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(f => f.UploadedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(f => f.UploadedAt <= toDate.Value);
        }

        return await query.OrderByDescending(f => f.UploadedAt).ToListAsync();
    }

    public async Task<FileRecord> UploadFileAsync(IFormFile file, string uploadedBy, string? description = null, string? tags = null, string category = "Documents")
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty or null");

        // Generate unique filename
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(_uploadsPath, uniqueFileName);

        // Create directory structure by date
        var dateFolder = DateTime.Now.ToString("yyyy/MM");
        var fullDirectoryPath = Path.Combine(_uploadsPath, dateFolder);
        if (!Directory.Exists(fullDirectoryPath))
        {
            Directory.CreateDirectory(fullDirectoryPath);
        }

        var fullFilePath = Path.Combine(fullDirectoryPath, uniqueFileName);
        var relativePath = Path.Combine("uploads", dateFolder, uniqueFileName).Replace("\\", "/");

        // Save file to disk
        using (var stream = new FileStream(fullFilePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Calculate file hash
        var fileHash = await CalculateFileHashAsync(fullFilePath);

        // Determine correct MIME type based on file extension
        var mimeType = fileExtension.ToLowerInvariant() switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".txt" => "text/plain",
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            ".7z" => "application/x-7z-compressed",
            _ => file.ContentType ?? "application/octet-stream"
        };

        // Create file record
        var fileRecord = new FileRecord
        {
            FileName = uniqueFileName,
            OriginalFileName = file.FileName,
            FilePath = relativePath,
            FileExtension = fileExtension,
            MimeType = mimeType,
            FileSize = file.Length,
            Description = description,
            Tags = tags,
            Category = category,
            UploadedBy = uploadedBy,
            UploadedAt = DateTime.UtcNow,
            FileHash = fileHash
        };

        _context.FileRecords.Add(fileRecord);
        await _context.SaveChangesAsync();

        // Log upload activity
        await LogActivityAsync(fileRecord.Id, "Upload", uploadedBy, $"File '{file.FileName}' uploaded");

        _logger.LogInformation($"File uploaded: {file.FileName} by {uploadedBy}");
        return fileRecord;
    }

    public async Task<FileRecord> UpdateFileAsync(FileRecord fileRecord)
    {
        fileRecord.ModifiedAt = DateTime.UtcNow;
        _context.FileRecords.Update(fileRecord);
        await _context.SaveChangesAsync();

        await LogActivityAsync(fileRecord.Id, "Update", fileRecord.ModifiedBy ?? "System", "File metadata updated");
        return fileRecord;
    }

    public async Task<FileRecord> UpdateLastAccessedTimeAsync(FileRecord fileRecord)
    {
        fileRecord.LastAccessedAt = DateTime.UtcNow;
        _context.FileRecords.Update(fileRecord);
        await _context.SaveChangesAsync();
        // Note: No activity logging for LastAccessedAt updates
        return fileRecord;
    }

    public async Task<bool> DeleteFileAsync(int id, string? performedBy = null, string? ipAddress = null, string? userAgent = null)
    {
        var fileRecord = await GetFileByIdAsync(id);
        if (fileRecord == null) return false;

        // Get file path before soft delete
        var filePath = await GetFilePathAsync(id);
        
        // Use provided performedBy or default to "System"
        var deletedBy = !string.IsNullOrEmpty(performedBy) ? performedBy : "System";
        
        // Soft delete
        fileRecord.IsActive = false;
        fileRecord.ModifiedAt = DateTime.UtcNow;
        fileRecord.ModifiedBy = deletedBy;
        
        await _context.SaveChangesAsync();
        
        // Delete physical file if it exists
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                _logger.LogInformation($"Physical file deleted: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting physical file: {filePath}");
                // Continue execution even if physical file deletion fails
            }
        }
        
        // Log delete activity with proper context
        await LogActivityAsync(id, "Delete", deletedBy, 
            $"File '{fileRecord.OriginalFileName}' deleted (including physical file)", 
            ipAddress, userAgent);

        _logger.LogInformation($"File deleted: {fileRecord.OriginalFileName} (ID: {id}) by {deletedBy}");
        return true;
    }

    public async Task<string> GetFilePathAsync(int id)
    {
        var fileRecord = await GetFileByIdAsync(id);
        if (fileRecord == null) return string.Empty;

        var fullPath = Path.Combine(_environment.WebRootPath, fileRecord.FilePath.Replace("/", "\\"));
        return File.Exists(fullPath) ? fullPath : string.Empty;
    }

    public async Task<bool> FileExistsAsync(int id)
    {
        var filePath = await GetFilePathAsync(id);
        return !string.IsNullOrEmpty(filePath) && File.Exists(filePath);
    }

    public async Task<IEnumerable<FileActivity>> GetFileActivitiesAsync(int fileId)
    {
        return await _context.FileActivities
            .Where(a => a.FileRecordId == fileId)
            .OrderByDescending(a => a.PerformedAt)
            .ToListAsync();
    }

    public async Task LogActivityAsync(int fileId, string activityType, string performedBy, string? description = null, string? ipAddress = null, string? userAgent = null)
    {
        try
        {
            _logger.LogInformation($"Logging activity: FileId={fileId}, Type={activityType}, User={performedBy}, IP={ipAddress ?? "NULL"}, UserAgent={userAgent ?? "NULL"}");
            
            var activity = new FileActivity
            {
                FileRecordId = fileId,
                ActivityType = activityType,
                Description = description,
                PerformedBy = performedBy,
                PerformedAt = DateTime.UtcNow,
                IpAddress = !string.IsNullOrEmpty(ipAddress) && ipAddress != "Unknown" ? ipAddress : null,
                UserAgent = !string.IsNullOrEmpty(userAgent) && userAgent != "Unknown" ? userAgent : null
            };

            _context.FileActivities.Add(activity);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Activity logged successfully with ID: {activity.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error logging activity for FileId={fileId}, Type={activityType}");
            throw;
        }
    }

    public async Task<IEnumerable<FileCategory>> GetCategoriesAsync()
    {
        return await _context.FileCategories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Dictionary<string, object>> GetDashboardStatsAsync()
    {
        var totalFiles = await _context.FileRecords.CountAsync(f => f.IsActive);
        var totalSize = await _context.FileRecords.Where(f => f.IsActive).SumAsync(f => f.FileSize);
        var todayUploads = await _context.FileRecords.CountAsync(f => f.IsActive && f.UploadedAt.Date == DateTime.UtcNow.Date);
        var totalDownloads = 0; // Downloads tracking removed
        
        var categoryStats = await _context.FileRecords
            .Where(f => f.IsActive)
            .GroupBy(f => f.Category)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToListAsync();

        var recentActivitiesQuery = await _context.FileActivities
            .Include(a => a.FileRecord)
            .OrderByDescending(a => a.PerformedAt)
            .Take(10)
            .ToListAsync();
            // Note: Show all activities including deleted files for dashboard

        // Get user names for recent activities
        var recentUserEmails = recentActivitiesQuery.Select(a => a.PerformedBy).Distinct().ToList();
        var recentUsers = await _context.Users
            .Where(u => recentUserEmails.Contains(u.Email))
            .Select(u => new { u.Email, u.Name })
            .ToListAsync();

        var recentUserNameDict = recentUsers.ToDictionary(u => u.Email, u => u.Name);

        var recentActivities = recentActivitiesQuery.Select(a => new FileActivityWithUser
        {
            Id = a.Id,
            FileRecordId = a.FileRecordId,
            ActivityType = a.ActivityType,
            Description = a.Description,
            PerformedBy = a.PerformedBy,
            PerformedByName = recentUserNameDict.GetValueOrDefault(a.PerformedBy, a.PerformedBy),
            PerformedAt = a.PerformedAt,
            IpAddress = a.IpAddress,
            UserAgent = a.UserAgent,
            FileRecord = a.FileRecord
        }).ToList();

        return new Dictionary<string, object>
        {
            ["totalFiles"] = totalFiles,
            ["totalSize"] = totalSize,
            ["todayUploads"] = todayUploads,
            ["totalDownloads"] = totalDownloads,
            ["categoryStats"] = categoryStats,
            ["recentActivities"] = recentActivities
        };
    }

    public async Task<ActivitiesResult> GetAllActivitiesWithUsersAsync(string? searchTerm = null, string? activityType = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 50)
    {
        // Start with base query including all activities (not filtering by IsActive)
        IQueryable<FileActivity> query = _context.FileActivities
            .Include(a => a.FileRecord);
            // Note: Removed IsActive filter to show delete logs

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a => 
                a.FileRecord.OriginalFileName.Contains(searchTerm) ||
                a.PerformedBy.Contains(searchTerm) ||
                (a.Description != null && a.Description.Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(activityType))
        {
            query = query.Where(a => a.ActivityType == activityType);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(a => a.PerformedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            var endDate = toDate.Value.AddDays(1);
            query = query.Where(a => a.PerformedAt < endDate);
        }

        var totalCount = await query.CountAsync();

        var activities = await query
            .OrderByDescending(a => a.PerformedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Get all unique user emails from the activities
        var userEmails = activities.Select(a => a.PerformedBy).Distinct().ToList();
        
        // Get user names from the database
        var users = await _context.Users
            .Where(u => userEmails.Contains(u.Email))
            .Select(u => new { u.Email, u.Name })
            .ToListAsync();

        var userNameDict = users.ToDictionary(u => u.Email, u => u.Name);

        // Map to FileActivityWithUser
        var activitiesWithUsers = activities.Select(a => new FileActivityWithUser
        {
            Id = a.Id,
            FileRecordId = a.FileRecordId,
            ActivityType = a.ActivityType,
            Description = a.Description,
            PerformedBy = a.PerformedBy,
            PerformedByName = userNameDict.GetValueOrDefault(a.PerformedBy, a.PerformedBy),
            PerformedAt = a.PerformedAt,
            IpAddress = a.IpAddress,
            UserAgent = a.UserAgent,
            FileRecord = a.FileRecord
        }).ToList();

        return new ActivitiesResult
        {
            Activities = activitiesWithUsers,
            TotalCount = totalCount
        };
    }

    private async Task<string> CalculateFileHashAsync(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        var hash = await md5.ComputeHashAsync(stream);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}