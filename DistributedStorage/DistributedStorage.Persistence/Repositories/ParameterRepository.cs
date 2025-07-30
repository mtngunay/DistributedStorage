using DistributedStorage.Application.Abstractions.Repositories;
using DistributedStorage.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Parameter = DistributedStorage.Domain.Entities.Parameter;

namespace DistributedStorage.Persistence.Repositories
{
    public class ParameterRepository : GenericRepository<Parameter>, IParameterRepository
    {
        private readonly AppDbContext _context;

        public ParameterRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<string?> GetValueByKeyAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _context.Parameters
                .Where(p => p.Key == key)
                .Select(p => p.Value)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}