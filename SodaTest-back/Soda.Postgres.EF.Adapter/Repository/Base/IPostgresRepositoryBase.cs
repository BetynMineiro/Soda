using System.Linq.Expressions;
using Soda.Domain.Entities.Base;

namespace Soda.Postgres.EF.Adapter.Repository.Base;

public interface IPostgresRepositoryBase<T> where T : EntityBase
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<T?> GetSingleOrDefaultAsync(
        Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    IQueryable<T> GetAllAsync();
    Task InsertAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}