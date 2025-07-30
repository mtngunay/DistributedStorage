namespace DistributedStorage.Application.Abstractions.Services
{
    public interface IFileDownloadService
    {
        Task<byte[]> DownloadFileAsync(Guid fileId, CancellationToken cancellationToken = default);
    }
}