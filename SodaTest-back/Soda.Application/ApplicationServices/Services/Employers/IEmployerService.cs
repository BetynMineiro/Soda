using Soda.Application.ApplicationServices.Services.Employers.Payload;
using Soda.Application.ApplicationServices.Services.Employers.Payload.Request;
using Soda.Application.ApplicationServices.Services.Employers.Payload.Response;
using Soda.CrossCutting.RequestObjects;
using Soda.CrossCutting.ResultObjects;
using Soda.Domain.Entities;

namespace Soda.Application.ApplicationServices.Services.Employers;

public interface IEmployerService
{
    Task<EmployerResponse?> CreateAsync(CreateEmployerRequest request, CancellationToken cancellationToken);
    Task<EmployerResponse?> UpdateAsync(UpdateEmployerRequest request, CancellationToken cancellationToken);
    Task<EmployerResponse?> DeleteAsync(DeleteEmployerRequest request, CancellationToken cancellationToken);
    Task<Employer?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Pagination<Employer>> GetAllAsync(PagedRequest request,CancellationToken cancellationToken);
}