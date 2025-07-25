using Soda.Domain.Entities;

namespace Soda.Application.ApplicationServices.Services.Employers.Payload.Request;

public class CreateEmployerRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TaxDocument { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; } = DateOnly.FromDateTime(DateTime.MinValue);
    public string? ManagerId { get; set; } = string.Empty;
    public string? ExternalId { get; set; } = string.Empty;
    public EmployerType Type { get; set; } = EmployerType.Employee;
    
    public List<PhoneEmployer>? PhoneNumbers { get; set; }
    public string Password { get; set; }= string.Empty;
}