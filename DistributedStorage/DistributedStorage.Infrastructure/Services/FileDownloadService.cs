using DistributedStorage.Application.Abstractions.Services;
using DistributedStorage.Application.Abstractions.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace DistributedStorage.Infrastructure.Services
{
    public class FileDownloadService : IFileDownloadService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageProvider _storageProvider;
        private readonly IChecksumService _checksumService;
        private readonly ILogger<FileDownloadService> _logger;

        public FileDownloadService(IUnitOfWork unitOfWork, IStorageProvider storageProvider, ILogger<FileDownloadService> logger, IChecksumService checksumService)
        {
            _unitOfWork = unitOfWork;
            _storageProvider = storageProvider;
            _logger = logger;
            _checksumService = checksumService;
        }

        public async Task<byte[]> DownloadFileAsync(Guid fileId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Dosya indirme başladı: {FileId}", fileId);

            var chunks = await _unitOfWork.ChunkRepository
                .GetAllByFileIdAsync(fileId, cancellationToken);

            if (!chunks.Any())
            {
                _logger.LogWarning("Dosya için chunk bulunamadı: {FileId}", fileId);
                return Array.Empty<byte>();
            }

            using var ms = new MemoryStream();

            foreach (var chunk in chunks.OrderBy(c => c.ChunkNumber))
            {
                var chunkData = await _storageProvider.GetChunkAsync(chunk.Id, decrypt: true, cancellationToken);

                if (!string.IsNullOrEmpty(chunk.Checksum))
                {
                    var isValid = _checksumService.VerifyHash(chunkData, chunk.Checksum);
                    if (!isValid)
                    {
                        _logger.LogError("Checksum doğrulaması başarısız. ChunkId: {ChunkId}", chunk.Id);
                        throw new InvalidOperationException($"Checksum verification failed for chunk: {chunk.Id}");
                    }
                }

                await ms.WriteAsync(chunkData, 0, chunkData.Length, cancellationToken);
            }

            _logger.LogInformation("Dosya indirme tamamlandı: {FileId}", fileId);
            return ms.ToArray();
        }
    }
}