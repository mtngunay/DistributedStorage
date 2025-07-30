using Parameter = DistributedStorage.Domain.Entities.Parameter;

namespace DistributedStorage.Application.Abstractions.Repositories
{
    public interface IParameterRepository : IGenericRepository<Parameter>
    {
        Task<string?> GetValueByKeyAsync(string key, CancellationToken cancellationToken = default);
    }
}