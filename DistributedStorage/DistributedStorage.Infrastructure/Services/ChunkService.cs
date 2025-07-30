using DistributedStorage.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace DistributedStorage.Infrastructure.Services
{
    public class ChunkService : IChunkService
    {
        private readonly IStorageProvider _storageProvider;
        private readonly IChecksumService _checksumService;
        private readonly ILogger<ChunkService> _logger;

        public ChunkService(IStorageProvider storageProvider, IChecksumService checksumService, ILogger<ChunkService> logger)
        {
            _storageProvider = storageProvider;
            _checksumService = checksumService;
            _logger = logger;
        }

        public async Task SaveChunkAsync(Guid chunkId, byte[] data, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Kaydedilen chunkId: {chunkId}");
            await _storageProvider.SaveChunkAsync(chunkId, data, cancellationToken: cancellationToken);
        }

        public async Task<byte[]> GetChunkAsync(Guid chunkId, CancellationToken cancellationToken = default)
        {
            return await _storageProvider.GetChunkAsync(chunkId, cancellationToken: cancellationToken);
        }

        public async Task DeleteChunkAsync(Guid chunkId, CancellationToken cancellationToken = default)
        {
            await _storageProvider.DeleteChunkAsync(chunkId, cancellationToken: cancellationToken);
        }

        public async Task<bool> VerifyChecksumAsync(Guid chunkId, string expectedChecksum, CancellationToken cancellationToken = default)
        {
            var data = await _storageProvider.GetChunkAsync(chunkId);
            var actualChecksum = _checksumService.ComputeHash(data);
            return expectedChecksum.Equals(actualChecksum, StringComparison.OrdinalIgnoreCase);
        }
    }
}