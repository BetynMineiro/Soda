using Soda.Domain.Entities.Base;
using Soda.Postgres.EF.Adapter.Repository;
using Soda.Postgres.EF.Adapter.Repository.Base;


namespace Soda.Postgres.EF.Adapter.UnitOfWork;
public interface IUnitOfWork : IDisposable
{
    IPostgresRepositoryBase<T> Repository<T>() where T : EntityBase;

    Task BeginTransactionAsync(CancellationToken cancellationToken);
    Task CommitAsync(CancellationToken cancellationToken);
    Task RollbackAsync(CancellationToken cancellationToken);
}
