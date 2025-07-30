using DistributedStorage.Application.Abstractions.Repositories;
using DistributedStorage.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using File = DistributedStorage.Domain.Entities.File;

namespace DistributedStorage.Persistence.Repositories
{
    public class FileRepository : GenericRepository<File>, IFileRepository
    {
        public FileRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<File?> GetFileWithChunksAsync(Guid fileId, CancellationToken cancellationToken = default)
        {
            return await _context.Files
                .Include(f => f.Chunks)
                    .ThenInclude(c => c.ChunkBlob)
                .FirstOrDefaultAsync(f => f.Id == fileId, cancellationToken);
        }
    }
}