namespace DistributedStorage.Application.Abstractions.Services
{
    public interface IChunkService
    {
        Task SaveChunkAsync(Guid chunkId, byte[] data, CancellationToken cancellationToken = default);

        Task<byte[]> GetChunkAsync(Guid chunkId, CancellationToken cancellationToken = default);

        Task DeleteChunkAsync(Guid chunkId, CancellationToken cancellationToken = default);

        Task<bool> VerifyChecksumAsync(Guid chunkId, string expectedChecksum, CancellationToken cancellationToken = default);
    }
}