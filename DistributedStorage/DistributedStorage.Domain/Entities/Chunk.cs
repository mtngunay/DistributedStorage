using DistributedStorage.Domain.Common;
using DistributedStorage.Domain.Enums;

namespace DistributedStorage.Domain.Entities
{
    public class Chunk : BaseEntity
    {
        public Guid FileId { get; set; }
        public int ChunkNumber { get; set; }
        public string Checksum { get; set; } = string.Empty;
        public long Size { get; set; }
        public StorageType StorageType { get; set; }

        public File File { get; set; } = null!;
        public ChunkBlob? ChunkBlob { get; set; }
    }
}