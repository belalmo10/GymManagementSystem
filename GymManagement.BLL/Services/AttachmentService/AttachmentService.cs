using GymManagement.BLL.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;


namespace GymManagement.BLL.Services.AttachmentService
{
    public class AttachmentService : IAttachmentService
    {
        private readonly long _maxFileSize = 5 * 1024 * 1024;
        private readonly string[] _allowedExtensions = { ".jpg" , ".jpeg" , ".png" };

        private readonly ILogger<AttachmentService> _logger;
        private readonly IWebHostEnvironment _env;

        public AttachmentService(ILogger<AttachmentService> logger , IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }

        public Result Delete(string fileName , string folderName)
        {

            if ( string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName) ) return Result.NotFound("File Not Found");

            try
            {
                var fullPath = Path.Combine(_env.WebRootPath , folderName , fileName);

                if ( !File.Exists(fullPath) ) return Result.NotFound("File Not Exist");
                File.Delete(fullPath);
                return Result.Ok();
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex , "Failed to delete attachment {File}." , fileName);
                return Result.Fail("Failed To Delete Attachment File");
            }
        }

        public async Task<Result<string>> UploadAsync(Stream fileStream , string fileName , string folderName , CancellationToken ct = default)
        {
            if ( fileStream is null || !fileStream.CanRead ) return Result<string>.NotFound("Image Not Found");

            if ( fileStream.Length == 0 ) return Result<string>.Fail("File Length Can not be Less than Zero");

            if ( fileStream.Length > _maxFileSize )
            {
                _logger.LogError("Rejected upload: file too large ({Size} bytes)." , fileStream.Length);
                return Result<string>.Fail($"Rejected Upload: File Too Large ({fileStream.Length} bytes).");
            }

            var extension = Path.GetExtension(fileName);
            if ( string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension) )
            {
                _logger.LogWarning("Rejected upload: extension {Ext} not allowed." , extension);
                return Result<string>.Fail($"Rejected upload: extension {extension} not allowed.");
            }

            var uploadsFolder = Path.Combine(_env.ContentRootPath , folderName);
            Directory.CreateDirectory(uploadsFolder);

            var storedFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder , storedFileName);

            try
            {
                await using var fs = new FileStream(filePath , FileMode.CreateNew , FileAccess.Write , FileShare.None);
                await fileStream.CopyToAsync(fs , ct);
                return Result<string>.Ok(storedFileName);
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex , "Failed to upload file {fileName}." , fileName);
                return Result<string>.Fail("Failed to upload file");
            }
        }
        public (Stream Stream , string ContentType)? GetFile(string fileName , string folderName)
        {
            if ( string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName) ) return null;

            var fullPath = Path.Combine(_env.ContentRootPath , folderName , fileName);

            if ( !File.Exists(fullPath) ) return null;

            var contentType = Path.GetExtension(fullPath).ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            var stream = new FileStream(fullPath , FileMode.Open , FileAccess.Read , FileShare.Read);
            return (stream , contentType);
        }
    }
}
