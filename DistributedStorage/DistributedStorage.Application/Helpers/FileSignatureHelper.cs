namespace DistributedStorage.Application.Helpers
{
    public static class FileSignatureHelper
    {
        public static bool ValidateFileSignature(ReadOnlySpan<byte> fileContent, string expectedMimeType)
        {
            if (fileContent.Length < 4) return false;

            return expectedMimeType switch
            {
                "application/pdf" => fileContent.StartsWith(new byte[] { 0x25, 0x50, 0x44, 0x46 }),
                "image/png" => fileContent.StartsWith(new byte[] { 0x89, 0x50, 0x4E, 0x47 }),
                "image/jpeg" => fileContent.StartsWith(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }) ||
                                fileContent.StartsWith(new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 }),
                _ => false,
            };
        }
    }
}