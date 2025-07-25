using FluentValidation;
using Soda.Domain.Models.Authentication.SignUp;

namespace Soda.Domain.Specification.Authentication;

public interface ISignUpSpecification
{
    void AddRuleEmailShouldNotEmpty(AbstractValidator<DomainServices.Authentication.Interfaces.Authentication.SignUpModel> validator);
    void AddRuleNameShouldNotEmpty(AbstractValidator<DomainServices.Authentication.Interfaces.Authentication.SignUpModel> validator);
    void AddRulePasswordShouldNotEmpty(AbstractValidator<DomainServices.Authentication.Interfaces.Authentication.SignUpModel> validator);
    void AddRuleEmailShouldBeValid(AbstractValidator<DomainServices.Authentication.Interfaces.Authentication.SignUpModel> validator);
    void AddRulePasswordShouldBeStrong(AbstractValidator<DomainServices.Authentication.Interfaces.Authentication.SignUpModel> validator);
    
}
public class SignUpSpecification : ISignUpSpecification
{

    public void AddRuleEmailShouldNotEmpty(AbstractValidator<DomainServices.Authentication.Interfaces.Authentication.SignUpModel> validator)
    {
        validator.RuleFor(entity => entity.Email)
            .NotEmpty().WithMessage("Email is required");
    }

    public void AddRuleNameShouldNotEmpty(AbstractValidator<DomainServices.Authentication.Interfaces.Authentication.SignUpModel> validator)
    {
        validator.RuleFor(entity => entity.Name)
            .NotEmpty().WithMessage("Name is required");
    }

    public void AddRulePasswordShouldNotEmpty(AbstractValidator<DomainServices.Authentication.Interfaces.Authentication.SignUpModel> validator)
    {
        validator.RuleFor(entity => entity.Password)
            .NotEmpty().WithMessage("Password is required");
    }

    public void AddRuleEmailShouldBeValid(AbstractValidator<DomainServices.Authentication.Interfaces.Authentication.SignUpModel> validator)
    {
        validator.RuleFor(entity => entity.Email)
            .EmailAddress().WithMessage("Email is not valid");
    }

    public void AddRulePasswordShouldBeStrong(AbstractValidator<DomainServices.Authentication.Interfaces.Authentication.SignUpModel> validator)
    {
        validator.RuleFor(entity => entity.Password)
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter") 
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
    }
}