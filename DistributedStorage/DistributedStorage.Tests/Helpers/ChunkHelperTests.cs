using FluentAssertions;

namespace DistributedStorage.Tests.Helpers
{
    public class ChunkHelperTests
    {
        [Fact]
        public void SplitFileIntoChunks_ShouldSplitCorrectly()
        {
            var testBytes = new byte[1024 * 1024];
            new Random().NextBytes(testBytes);

            var chunkSize = 2 * 1024 * 1024;

            var chunks = ChunkHelper.SplitBytesToChunks(testBytes, chunkSize);

            chunks.Should().NotBeEmpty();
            chunks.Sum(c => c.Length).Should().Be(testBytes.Length);
            chunks.Count.Should().Be((int)Math.Ceiling((double)testBytes.Length / chunkSize));
        }

        [Theory]
        [InlineData(1024, 256)]
        [InlineData(1000, 100)]
        [InlineData(512, 512)]
        [InlineData(100, 200)]
        public void SplitBytesToChunks_ShouldSplitCorrectly(int dataSize, int chunkSize)
        {
            var data = Enumerable.Range(0, dataSize).Select(i => (byte)(i % 256)).ToArray();

            var chunks = ChunkHelper.SplitBytesToChunks(data, chunkSize);

            chunks.Should().NotBeNull();
            chunks.SelectMany(b => b).ToArray().Should().BeEquivalentTo(data);

            int expectedChunkCount = (int)Math.Ceiling((double)dataSize / chunkSize);
            chunks.Count.Should().Be(expectedChunkCount);
        }

        [Fact]
        public void MergeChunksToBytes_ShouldReconstructOriginalData()
        {
            var originalData = Enumerable.Range(0, 1024).Select(i => (byte)(i % 256)).ToArray();
            int chunkSize = 128;
            var chunks = ChunkHelper.SplitBytesToChunks(originalData, chunkSize);

            var merged = ChunkHelper.MergeChunksToBytes(chunks);

            merged.Should().BeEquivalentTo(originalData);
        }

        [Fact]
        public void SplitAndMerge_ShouldPreserveOriginalData()
        {
            var data = Enumerable.Range(0, 1500).Select(i => (byte)(i % 256)).ToArray();
            int chunkSize = 512;

            var chunks = ChunkHelper.SplitBytesToChunks(data, chunkSize);
            var merged = ChunkHelper.MergeChunksToBytes(chunks);

            merged.Should().BeEquivalentTo(data);
        }

        [Fact]
        public void SplitBytesToChunks_ShouldReturnOneChunk_WhenDataIsSmallerThanChunkSize()
        {
            byte[] data = new byte[10];
            int chunkSize = 100;

            var chunks = ChunkHelper.SplitBytesToChunks(data, chunkSize);

            chunks.Should().HaveCount(1);
            chunks[0].Length.Should().Be(10);
        }
    }
}