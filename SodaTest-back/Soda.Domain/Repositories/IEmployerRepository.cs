using Soda.Domain.Entities;

namespace Soda.Domain.Repositories;

public interface IEmployerRepository
{
    Task<Employer?> GetByExternalIdAsync(string id,  CancellationToken cancellationToken);
    Task<Employer?> GetByTaxDocumentAsync(string document,  CancellationToken cancellationToken);
    
}