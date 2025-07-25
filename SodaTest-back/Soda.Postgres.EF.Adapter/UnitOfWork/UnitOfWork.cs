using Microsoft.EntityFrameworkCore.Storage;
using Soda.Domain.Entities.Base;
using Soda.Postgres.EF.Adapter.Context;
using Soda.Postgres.EF.Adapter.Repository;
using Soda.Postgres.EF.Adapter.Repository.Base;

namespace Soda.Postgres.EF.Adapter.UnitOfWork;

public class UnitOfWork(PostgresDbContext context) : IUnitOfWork
{
    private readonly Dictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _transaction;

    public IPostgresRepositoryBase<T> Repository<T>() where T : EntityBase
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new PostgresRepositoryBase<T>(context);
        }

        return (IPostgresRepositoryBase<T>)_repositories[type];
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default )
    {
        if (_transaction == null)
        {
            _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        }
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default )
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default )
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }
}