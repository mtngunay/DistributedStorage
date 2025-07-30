using DistributedStorage.Application.Abstractions.Repositories;
using DistributedStorage.Domain.Entities;
using DistributedStorage.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DistributedStorage.Persistence.Repositories
{
    public class ChunkRepository : GenericRepository<Chunk>, IChunkRepository
    {
        private readonly AppDbContext _context;

        public ChunkRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Chunk>> GetAllByFileIdAsync(Guid fileId, CancellationToken cancellationToken = default)
        {
            return await _context.Chunks
                .Where(c => c.FileId == fileId)
                .OrderBy(c => c.ChunkNumber)
                .ToListAsync(cancellationToken);
        }
    }
}