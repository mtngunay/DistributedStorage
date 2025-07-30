using DistributedStorage.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DistributedStorage.Persistence.Seeders
{
    public class DbSeeder
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DbSeeder> _logger;

        public DbSeeder(AppDbContext context, ILogger<DbSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Seed işlemi başlatıldı...");
                var seedData = new List<DistributedStorage.Domain.Entities.Parameter>
            {
                new()
                {
                    Id = new Guid("d9211026-d96c-4950-912e-ec2c48071b08"),
                    Key = "DefaultChunkSizePercent",
                    Value = "1",
                    CreatedAt = DateTime.UtcNow,
                },
                new()
                {
                    Id = new Guid("21ac0ad6-49fa-41f8-bd76-ed29cafbc1d0"),
                    Key = "AllowedMimeTypes",
                    Value = "application/pdf,image/png,image/jpeg",
                    CreatedAt = DateTime.UtcNow,
                },
            };

                foreach (var param in seedData)
                {
                    var exists = await _context
                        .Parameters.AsNoTracking()
                        .AnyAsync(p => p.Key == param.Key, cancellationToken);

                    if (!exists)
                    {
                        _context.Parameters.Add(param);
                        _logger.LogInformation("Seed parametresi eklendi: {Key}", param.Key);
                    }
                    else
                    {
                        _logger.LogInformation("Parametre zaten mevcut, atlandı: {Key}", param.Key);
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Seed işlemi tamamlandı.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Seed işlemi sırasında hata oluştu.");
                throw;
            }
        }
    }
}