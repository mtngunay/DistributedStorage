using DistributedStorage.Application.Abstractions.Services;
using DistributedStorage.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DistributedStorage.Infrastructure.DIs
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddScoped<IStorageProvider, FileSystemStorageProvider>();
            services.AddScoped<IChecksumService, ChecksumService>();
            services.AddScoped<IFileDownloadService, FileDownloadService>();

            return services;
        }
    }
}