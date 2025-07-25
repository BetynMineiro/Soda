using Soda.Domain.Entities.Base;

namespace Soda.Domain.Entities;

public class Employer : EntityBase
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string TaxDocument { get; set; }
    public string Email { get; set; }
    public string? ExternalId { get; set; }
    public DateOnly BirthDate { get; set; }

    public int Age
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var age = today.Year - BirthDate.Year;

            if (BirthDate > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }

    public EmployerType Type { get; set; } = EmployerType.Employee;
    public Guid? ManagerId { get; set; }
    public virtual Employer? Manager { get; set; }
    public virtual ICollection<PhoneEmployer>? PhoneNumbers { get; set; }
    public string? Avatar { get; set; }
}

public enum EmployerType
{
    Employee,
    Developer,
    TeamLead,
    Manager,
    Cfo,
    Admin
}