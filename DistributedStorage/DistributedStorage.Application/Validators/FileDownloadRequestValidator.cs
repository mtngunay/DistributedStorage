using DistributedStorage.Application.DTOs;
using FluentValidation;

namespace DistributedStorage.Application.Validators
{
    public class FileDownloadRequestValidator : AbstractValidator<FileDownloadRequest>
    {
        public FileDownloadRequestValidator()
        {
            RuleFor(x => x.FileId)
                .NotEmpty().WithMessage("FileId boş olamaz.");

            RuleFor(x => x.DestinationPath)
                .NotEmpty().WithMessage("İndirme yolu belirtilmelidir.")
                .Must(path => Directory.Exists(Path.GetDirectoryName(path)!))
                .WithMessage("Hedef dizin mevcut değil.");
        }
    }
}