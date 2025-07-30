using DistributedStorage.Application.Abstractions.Repositories;

namespace DistributedStorage.Application.Abstractions.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IParameterRepository ParameterRepository { get; }
        IFileRepository FileRepository { get; }
        IChunkRepository ChunkRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}