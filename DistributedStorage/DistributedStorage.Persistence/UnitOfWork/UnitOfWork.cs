using DistributedStorage.Application.Abstractions.Repositories;
using DistributedStorage.Application.Abstractions.UnitOfWork;
using DistributedStorage.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace DistributedStorage.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;
        public IParameterRepository ParameterRepository { get; }
        public IFileRepository FileRepository { get; }
        public IChunkRepository ChunkRepository { get; }

        public UnitOfWork(
            AppDbContext context,
            IParameterRepository parameterRepository,
            IFileRepository fileRepository,
            IChunkRepository chunkRepository
        )
        {
            _context = context;
            ParameterRepository = parameterRepository;
            FileRepository = fileRepository;
            ChunkRepository = chunkRepository;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
                return;

            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction başlatılmamış.");

            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
            await DisposeTransactionAsync();
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
                return;

            await _transaction.RollbackAsync(cancellationToken);
            await DisposeTransactionAsync();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task DisposeTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}