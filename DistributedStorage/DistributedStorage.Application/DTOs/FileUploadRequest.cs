namespace DistributedStorage.Application.DTOs
{
    public class FileUploadRequest
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long Size { get; set; }
        public byte[] Content { get; set; } = Array.Empty<byte>();
    }
}