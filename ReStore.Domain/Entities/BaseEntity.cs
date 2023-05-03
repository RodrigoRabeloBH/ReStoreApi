using System.Diagnostics.CodeAnalysis;

namespace ReStore.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseEntity
    {
        public int Id { get; set; }
    }
}
