namespace Soda.Domain.Entities;

public class PhoneEmployer
{
    public Guid Id { get; set; }
    public Guid EmployerId { get; set; }
    public string Number { get; set; }
    public virtual Employer Employer { get; set; }
}