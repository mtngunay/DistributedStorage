using DistributedStorage.Application.Abstractions.Services;
using DistributedStorage.Application.DTOs;
using DistributedStorage.Application.Validators;
using DistributedStorage.Infrastructure.DIs;
using DistributedStorage.Infrastructure.Services;
using DistributedStorage.Persistence.DIs;
using DistributedStorage.Persistence.Seeders;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

services.AddLogging(config =>
{
    config.AddConsole();
});

var connectionString = "Server=(LocalDb)\\MSSqlLocalDB;Database=DistributedStorageDb;Trusted_Connection=True;MultipleActiveResultSets=true";

services.AddPersistence(connectionString);

services.AddValidatorsFromAssemblyContaining<FileUploadRequestValidator>();

services.AddInfrastructure();

services.AddSingleton<IStorageProvider>(provider =>
    new FileSystemStorageProvider("ChunksStorage"));

var provider = services.BuildServiceProvider();
var logger = provider.GetRequiredService<ILogger<Program>>();

var uploadService = provider.GetRequiredService<IFileUploadService>();

using var scope = provider.CreateScope();

try
{
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await seeder.SeedAsync();
}
catch (Exception ex)
{
    logger.LogError(ex, "Seed işlemi sırasında bir hata oluştu.");
}

Console.WriteLine("=== Dosya Yükleme / İndirme Uygulaması ===");
Console.WriteLine("1 - Dosya Yükle");
Console.WriteLine("2 - Dosya İndir");
Console.Write("Seçiminiz: ");

var choice = Console.ReadLine();

switch (choice)
{
    case "1":
        await UploadFileAsync(provider, logger);
        break;

    case "2":
        await DownloadFileAsync(provider, logger);
        break;

    default:
        Console.WriteLine("Geçersiz seçim.");
        break;
}

Console.WriteLine("İşlem tamamlandı. Çıkmak için bir tuşa basın.");
Console.ReadKey();

static async Task UploadFileAsync(IServiceProvider provider, ILogger logger)
{
    var uploadService = provider.GetRequiredService<IFileUploadService>();
    var validator = provider.GetRequiredService<IValidator<FileUploadRequest>>();

    var filePath = @"C:\Temp\case.pdf";
    if (!System.IO.File.Exists(filePath))
    {
        logger.LogError("Dosya bulunamadı: {Path}", filePath);
        return;
    }

    var fileInfo = new FileInfo(filePath);
    var request = new FileUploadRequest
    {
        FileName = fileInfo.Name,
        MimeType = "application/pdf",
        Size = fileInfo.Length,
        FilePath = filePath,
        Content = File.ReadAllBytes(filePath)
    };

    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        foreach (var error in validationResult.Errors)
            logger.LogError("Validasyon hatası: {Error}", error.ErrorMessage);
        return;
    }

    await uploadService.UploadAsync(request, true);
    logger.LogInformation("Dosya başarıyla yüklendi.");
}

static async Task DownloadFileAsync(IServiceProvider provider, ILogger logger)
{
    var downloadService = provider.GetRequiredService<IFileDownloadService>();

    Console.Write("İndirilecek dosyanın ID'sini girin (Guid): ");
    var input = Console.ReadLine();

    if (!Guid.TryParse(input, out var fileId))
    {
        logger.LogError("Geçersiz GUID formatı.");
        return;
    }

    var content = await downloadService.DownloadFileAsync(fileId);

    if (content.Length == 0)
    {
        logger.LogWarning("İndirilen dosya içeriği boş.");
        return;
    }

    var outputPath = $@"C:\Temp\Downloaded_{fileId}.pdf";
    await File.WriteAllBytesAsync(outputPath, content);
    logger.LogInformation("Dosya başarıyla indirildi: {Path}", outputPath);
}