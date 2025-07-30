namespace DistributedStorage.Tests.Helpers
{
    public static class ChunkHelper
    {
        public static List<byte[]> SplitBytesToChunks(byte[] data, int chunkSize)
        {
            var chunks = new List<byte[]>();
            int totalSize = data.Length;
            for (int i = 0; i < totalSize; i += chunkSize)
            {
                int currentChunkSize = Math.Min(chunkSize, totalSize - i);
                byte[] chunk = new byte[currentChunkSize];
                Array.Copy(data, i, chunk, 0, currentChunkSize);
                chunks.Add(chunk);
            }
            return chunks;
        }

        public static byte[] MergeChunksToBytes(List<byte[]> chunks)
        {
            using var memory = new MemoryStream();
            foreach (var chunk in chunks)
                memory.Write(chunk, 0, chunk.Length);

            return memory.ToArray();
        }
    }
}