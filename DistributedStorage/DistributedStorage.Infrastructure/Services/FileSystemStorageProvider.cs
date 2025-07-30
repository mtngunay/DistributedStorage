using DistributedStorage.Application.Abstractions.Services;
using DistributedStorage.Infrastructure.Helpers;

namespace DistributedStorage.Infrastructure.Services
{
    public class FileSystemStorageProvider : IStorageProvider
    {
        private readonly string _storagePath;

        public FileSystemStorageProvider(string storagePath)
        {
            _storagePath = storagePath;
        }

        public async Task SaveChunkAsync(Guid chunkId, byte[] data, bool encrypt = false, CancellationToken cancellationToken = default)
        {
            var dataToSave = encrypt ? EncryptionHelper.Encrypt(data) : data;
            var filePath = Path.Combine(_storagePath, chunkId.ToString());
            await File.WriteAllBytesAsync(filePath, dataToSave, cancellationToken);
        }

        public async Task<byte[]> GetChunkAsync(Guid chunkId, bool decrypt = false, CancellationToken cancellationToken = default)
        {
            var filePath = Path.Combine(_storagePath, chunkId.ToString());
            var data = await File.ReadAllBytesAsync(filePath, cancellationToken);
            return decrypt ? EncryptionHelper.Decrypt(data) : data;
        }

        public Task DeleteChunkAsync(Guid chunkId, CancellationToken cancellationToken = default)
        {
            var filePath = Path.Combine(_storagePath, chunkId.ToString());
            if (File.Exists(filePath))
                File.Delete(filePath);
            return Task.CompletedTask;
        }
    }
}