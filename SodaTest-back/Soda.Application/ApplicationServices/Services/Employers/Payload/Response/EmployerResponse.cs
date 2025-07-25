namespace Soda.Application.ApplicationServices.Services.Employers.Payload.Response;

public class EmployerResponse(string message, Guid employerId)
{
    public string Message { get; } = message;
    public Guid EmployerId { get; } = employerId;
}