using DistributedStorage.Application.Abstractions.Repositories;
using DistributedStorage.Application.Abstractions.UnitOfWork;
using DistributedStorage.Persistence.Contexts;
using DistributedStorage.Persistence.Repositories;
using DistributedStorage.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DistributedStorage.Persistence.DIs
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
             options.UseSqlServer(connectionString));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IParameterRepository, ParameterRepository>();
            services.AddScoped<IChunkRepository, ChunkRepository>();

            services.AddTransient<DbSeeder>();

            return services;
        }
    }
}