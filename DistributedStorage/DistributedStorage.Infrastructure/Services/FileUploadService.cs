using DistributedStorage.Application.Abstractions.Services;
using DistributedStorage.Application.Abstractions.UnitOfWork;
using DistributedStorage.Application.DTOs;
using DistributedStorage.Domain.Entities;
using DistributedStorage.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace DistributedStorage.Infrastructure.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FileUploadService> _logger;
        private readonly IStorageProvider _storageProvider;

        public FileUploadService(
            IUnitOfWork unitOfWork,
            ILogger<FileUploadService> logger,
            IStorageProvider storageProvider)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _storageProvider = storageProvider;
        }

        public async Task UploadAsync(FileUploadRequest request, bool encryptChunks, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Dosya yükleme işlemi başlatıldı: {FileName}", request.FileName);

            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var chunkSizeParam = await _unitOfWork.ParameterRepository
                .GetValueByKeyAsync("DefaultChunkSizePercent", cancellationToken);

                if (chunkSizeParam == null || !decimal.TryParse(chunkSizeParam, out var percent))
                {
                    _logger.LogError(
                        "Parameter tablosunda 'DefaultChunkSizePercent' bulunamadı ya da geçersiz."
                    );
                    throw new InvalidOperationException("Chunk boyutu belirlenemedi. Parametre eksik.");
                }

                var chunkRatio = percent / 100m;
                var chunkSize = (long)(request.Size * chunkRatio);

                if (chunkSize <= 0)
                {
                    _logger.LogWarning(
                        "Hesaplanan chunk boyutu 0 veya negatif. Minimum 1KB olarak ayarlanıyor."
                    );
                    chunkSize = 1 * 1024;
                }

                long minChunkSize = 1024;

                if (chunkSize < minChunkSize)
                    chunkSize = Math.Min(minChunkSize, request.Size);

                if (chunkSize > request.Size)
                    chunkSize = request.Size;

                _logger.LogInformation("Chunk boyutu: {ChunkSize} byte", chunkSize);

                var file = new Domain.Entities.File
                {
                    Id = Guid.NewGuid(),
                    FileName = request.FileName,
                    MimeType = request.MimeType,
                    Size = request.Size,
                    ChunkCount = 0,
                    Checksum = ComputeSHA256(request.Content),
                };

                await _unitOfWork.FileRepository.AddAsync(file);

                int totalChunks = (int)Math.Ceiling((double)request.Content.Length / chunkSize);
                _logger.LogInformation(
                    "{FileName} dosyası {ChunkCount} parçaya ayrılacak.",
                    request.FileName,
                    totalChunks
                );

                for (int i = 0; i < totalChunks; i++)
                {
                    var chunkData = request
                        .Content.Skip(i * (int)chunkSize)
                        .Take((int)chunkSize)
                        .ToArray();

                    var chunk = new Chunk
                    {
                        Id = Guid.NewGuid(),
                        FileId = file.Id,
                        ChunkNumber = i + 1,
                        Size = chunkData.Length,
                        Checksum = ComputeSHA256(chunkData),
                        StorageType = StorageType.Database,
                        ChunkBlob = new ChunkBlob { Id = Guid.NewGuid(), Data = chunkData },
                    };

                    await _storageProvider.SaveChunkAsync(chunk.Id, chunkData, encryptChunks, cancellationToken);

                    await _unitOfWork.ChunkRepository.AddAsync(chunk);
                }

                file.ChunkCount = totalChunks;

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(
                    "{FileName} dosyası başarıyla {ChunkCount} parçaya bölündü.",
                    request.FileName,
                    totalChunks
                );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Dosya yükleme sırasında hata oluştu: {FileName}", request.FileName);
                throw;
            }
        }

        private static string ComputeSHA256(byte[] data)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(data);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}