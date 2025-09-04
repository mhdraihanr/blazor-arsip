using Microsoft.AspNetCore.Mvc;
using blazor_arsip.Services;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mime;

namespace blazor_arsip.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly ILogger<FileController> _logger;
    private readonly IWebHostEnvironment _environment;

    public FileController(IFileService fileService, ILogger<FileController> logger, IWebHostEnvironment environment)
    {
        _fileService = fileService;
        _logger = logger;
        _environment = environment;
    }

    [HttpGet("download/{id:int}")]
    public async Task<IActionResult> DownloadFile(int id)
    {
        try
        {
            var fileRecord = await _fileService.GetFileByIdAsync(id);
            if (fileRecord == null)
            {
                _logger.LogWarning($"File download attempted for non-existent file ID: {id}");
                return NotFound("File not found");
            }

            var filePath = Path.Combine(_environment.WebRootPath, fileRecord.FilePath.Replace("/", "\\"));
            if (!System.IO.File.Exists(filePath))
            {
                _logger.LogError($"Physical file not found: {filePath}");
                return NotFound("Physical file not found");
            }

            // Security check: Ensure file is within uploads directory
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            var fullFilePath = Path.GetFullPath(filePath);
            var fullUploadsPath = Path.GetFullPath(uploadsPath);
            
            if (!fullFilePath.StartsWith(fullUploadsPath, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning($"Attempted path traversal attack for file ID: {id}, Path: {filePath}");
                return Forbid("Access denied");
            }

            // Log download activity
            var userAgent = Request.Headers.UserAgent.ToString();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _fileService.LogActivityAsync(id, "Download", "Anonymous", 
                $"File '{fileRecord.OriginalFileName}' downloaded", ipAddress, userAgent);

            // Update download count
            fileRecord.DownloadCount++;
            fileRecord.LastAccessedAt = DateTime.UtcNow;
            await _fileService.UpdateFileAsync(fileRecord);

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var contentDisposition = new ContentDisposition
            {
                FileName = fileRecord.OriginalFileName,
                Inline = false
            };

            Response.Headers.Append("Content-Disposition", contentDisposition.ToString());
            
            _logger.LogInformation($"File downloaded: {fileRecord.OriginalFileName} (ID: {id})");
            
            // Determine correct MIME type based on file extension
            var mimeType = fileRecord.FileExtension.ToLowerInvariant() switch
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
                _ => fileRecord.MimeType ?? "application/octet-stream"
            };
            
            return File(fileBytes, mimeType, fileRecord.OriginalFileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error downloading file ID: {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("preview/{id:int}")]
    public async Task<IActionResult> PreviewFile(int id)
    {
        try
        {
            var fileRecord = await _fileService.GetFileByIdAsync(id);
            if (fileRecord == null)
            {
                return NotFound("File not found");
            }

            var filePath = Path.Combine(_environment.WebRootPath, fileRecord.FilePath.Replace("/", "\\"));
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Physical file not found");
            }

            // Security check: Ensure file is within uploads directory
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            var fullFilePath = Path.GetFullPath(filePath);
            var fullUploadsPath = Path.GetFullPath(uploadsPath);
            
            if (!fullFilePath.StartsWith(fullUploadsPath, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning($"Attempted path traversal attack for preview file ID: {id}, Path: {filePath}");
                return Forbid("Access denied");
            }

            // Only allow preview for safe file types
            var safePreviewTypes = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".txt" };
            if (!safePreviewTypes.Contains(fileRecord.FileExtension.ToLowerInvariant()))
            {
                return BadRequest("File type not supported for preview");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            
            // Set appropriate headers for file preview
            Response.Headers["Cache-Control"] = "public, max-age=3600";
            Response.Headers["Access-Control-Allow-Origin"] = "*";
            
            // Determine correct MIME type for preview
            var mimeType = fileRecord.FileExtension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain; charset=utf-8",
                _ => fileRecord.MimeType ?? "application/octet-stream"
            };
            
            return File(fileBytes, mimeType, enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error previewing file ID: {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        try
        {
            var result = await _fileService.DeleteFileAsync(id);
            if (!result)
            {
                return NotFound("File not found");
            }

            _logger.LogInformation($"File deleted via API: ID {id}");
            return Ok(new { message = "File deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting file ID: {id}");
            return StatusCode(500, "Internal server error");
        }
    }
}