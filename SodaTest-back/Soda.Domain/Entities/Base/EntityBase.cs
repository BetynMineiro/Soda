
namespace Soda.Domain.Entities.Base;

public abstract class EntityBase
{
    public DateTimeOffset CreatedAt { get;  set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get;  set; }
    public StatusEntity StatusEntity { get; set; } = StatusEntity.Active;

    public void SetUpdateInfo()=> UpdatedAt = DateTimeOffset.UtcNow;
}

public enum StatusEntity
{   Active = 1,
    Inactive = 0
}