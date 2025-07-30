using DistributedStorage.Application.Abstractions.Services;
using System.Security.Cryptography;
using System.Text;

namespace DistributedStorage.Infrastructure.Services
{
    public class ChecksumService : IChecksumService
    {
        private readonly HashAlgorithmName _algorithm;

        public ChecksumService(HashAlgorithmName? algorithm = null)
        {
            _algorithm = algorithm ?? HashAlgorithmName.SHA256;
        }

        public string ComputeHash(byte[] data)
        {
            using var hashAlgorithm = HashAlgorithm.Create(_algorithm.Name);
            var hash = hashAlgorithm!.ComputeHash(data);
            return FormatHash(hash);
        }

        public string ComputeHash(Stream stream)
        {
            if (stream.CanSeek)
                stream.Position = 0;

            using var hashAlgorithm = HashAlgorithm.Create(_algorithm.Name);
            var hash = hashAlgorithm!.ComputeHash(stream);

            if (stream.CanSeek)
                stream.Position = 0;

            return FormatHash(hash);
        }

        public bool VerifyHash(byte[] data, string expectedHash)
        {
            var actualHash = ComputeHash(data);
            return string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase);
        }

        public bool VerifyHash(Stream stream, string expectedHash)
        {
            var actualHash = ComputeHash(stream);
            return string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase);
        }

        private string FormatHash(byte[] hash)
        {
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}