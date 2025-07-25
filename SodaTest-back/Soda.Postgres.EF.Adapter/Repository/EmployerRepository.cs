using Soda.Domain.Entities;
using Soda.Domain.Repositories;
using Soda.Postgres.EF.Adapter.Context;
using Soda.Postgres.EF.Adapter.Repository.Base;

namespace Soda.Postgres.EF.Adapter.Repository;

public class EmployerRepository(PostgresDbContext context) : PostgresRepositoryBase<Employer>(context), IEmployerRepository
{
    public Task<Employer?> GetByExternalIdAsync(string id, CancellationToken cancellationToken)
    {
        return GetSingleOrDefaultAsync(c => c.ExternalId == id, cancellationToken);
    }

    public Task<Employer?> GetByTaxDocumentAsync(string document, CancellationToken cancellationToken)
    {
        return GetSingleOrDefaultAsync(c => c.TaxDocument == document, cancellationToken);
    }
}