using DistributedStorage.Domain.Common;

namespace DistributedStorage.Domain.Entities
{
    public class Parameter : BaseEntity
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}