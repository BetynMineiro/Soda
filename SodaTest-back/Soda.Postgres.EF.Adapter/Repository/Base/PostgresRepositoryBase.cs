using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Soda.Domain.Entities.Base;
using Soda.Postgres.EF.Adapter.Context;

namespace Soda.Postgres.EF.Adapter.Repository.Base;

public class PostgresRepositoryBase<T>(PostgresDbContext context) : IPostgresRepositoryBase<T>
    where T : EntityBase
{
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<T?> GetSingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = GetAllAsync();
        return await query.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public IQueryable<T> GetAllAsync() => _dbSet.AsQueryable();

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(id, cancellationToken);
    }

    public async Task InsertAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.FromResult(Task.CompletedTask);
    }


    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }
}