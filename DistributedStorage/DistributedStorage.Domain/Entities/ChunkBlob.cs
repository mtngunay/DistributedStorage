using DistributedStorage.Domain.Common;

namespace DistributedStorage.Domain.Entities
{
    public class ChunkBlob : BaseEntity
    {
        public Guid ChunkId { get; set; }
        public byte[] Data { get; set; } = Array.Empty<byte>();

        public Chunk Chunk { get; set; } = null!;
    }
}