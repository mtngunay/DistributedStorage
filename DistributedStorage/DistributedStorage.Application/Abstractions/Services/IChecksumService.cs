namespace DistributedStorage.Application.Abstractions.Services
{
    public interface IChecksumService
    {
        string ComputeHash(byte[] data);

        string ComputeHash(Stream stream);

        bool VerifyHash(byte[] data, string expectedHash);

        bool VerifyHash(Stream stream, string expectedHash);
    }
}