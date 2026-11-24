using GymManagement.BLL.Common;

namespace GymManagement.BLL.Services.AttachmentService
{
    public interface IAttachmentService
    {
        Task<Result<string>> UploadAsync(Stream fileStream , string fileName , string folderName , CancellationToken ct = default);
        Result Delete(string fileName , string folderName);
        (Stream Stream , string ContentType)? GetFile(string fileName , string folderName);
    }
}
