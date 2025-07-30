using DistributedStorage.Domain.Common;

namespace DistributedStorage.Domain.Entities
{
    public class File : BaseEntity
    {
        public string FileName { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long Size { get; set; }
        public string Checksum { get; set; } = string.Empty;
        public int ChunkCount { get; set; }

        public ICollection<Chunk> Chunks { get; set; } = new List<Chunk>();
    }
}