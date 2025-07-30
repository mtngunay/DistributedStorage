using DistributedStorage.Application.Abstractions.UnitOfWork;
using DistributedStorage.Application.DTOs;
using DistributedStorage.Application.Helpers;
using FluentValidation;

namespace DistributedStorage.Application.Validators
{
    public class FileUploadRequestValidator : AbstractValidator<FileUploadRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public FileUploadRequestValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.FileName).NotEmpty().WithMessage("Dosya adı boş olamaz.");

            RuleFor(x => x.MimeType)
                .NotEmpty()
                .WithMessage("MIME türü boş olamaz.")
                .MustAsync(IsMimeTypeAllowedAsync)
                .WithMessage("Dosya tipi izin verilen türler arasında değil.");

            RuleFor(x => x.Content)
                .NotNull()
                .WithMessage("Dosya içeriği boş olamaz.")
                .Must(
                    (request, content) =>
                        FileSignatureHelper.ValidateFileSignature(content, request.MimeType)
                )
                .WithMessage("Dosya içeriği MIME türü ile uyuşmuyor.");

            RuleFor(x => x.Size)
                .GreaterThan(0)
                .WithMessage("Dosya boyutu 0 olamaz.")
                .MustAsync(
                    async (size, cancellation) =>
                    {
                        var maxSizeStr = await _unitOfWork.ParameterRepository.GetValueByKeyAsync(
                            "MaxFileSize",
                            cancellation
                        );
                        if (string.IsNullOrEmpty(maxSizeStr))
                            return true;
                        if (!long.TryParse(maxSizeStr, out var maxSize))
                            return true;
                        return size <= maxSize;
                    }
                )
                .WithMessage("Dosya boyutu izin verilen maksimum boyutu aşıyor.");

            RuleFor(x => x.FilePath)
                .NotEmpty()
                .WithMessage("Dosya yolu boş olamaz.")
                .Must(File.Exists)
                .WithMessage("Dosya mevcut değil.");
        }

        private async Task<bool> IsMimeTypeAllowedAsync(string mimeType, CancellationToken cancellationToken)
        {
            var allowed = await _unitOfWork.ParameterRepository.GetValueByKeyAsync(
                "AllowedMimeTypes",
                cancellationToken
            );
            if (string.IsNullOrWhiteSpace(allowed))
                return false;

            var allowedList = allowed.Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            );
            return allowedList.Contains(mimeType);
        }
    }
}