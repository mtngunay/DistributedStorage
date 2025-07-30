using DistributedStorage.Application.DTOs;

namespace DistributedStorage.Application.Abstractions.Services
{
    public interface IFileUploadService
    {
        Task UploadAsync(FileUploadRequest request, bool encryptChunks, CancellationToken cancellationToken = default);
    }
}