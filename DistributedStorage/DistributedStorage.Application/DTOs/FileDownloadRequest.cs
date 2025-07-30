namespace DistributedStorage.Application.DTOs
{
    public class FileDownloadRequest
    {
        public Guid FileId { get; set; }
        public string? DestinationPath { get; set; } = "C:\\temp\\";
    }
}