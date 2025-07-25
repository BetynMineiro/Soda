using FluentValidation;

using Soda.Domain.Specification.Authentication;

namespace Soda.Domain.Validators.Authentication;

public interface IIsValidSignUpValidator : IValidator<DomainServices.Authentication.Interfaces.Authentication.SignUpModel>;
public class IsValidSignUpValidator : AbstractValidator<DomainServices.Authentication.Interfaces.Authentication.SignUpModel>,  IIsValidSignUpValidator
{
    public IsValidSignUpValidator(ISignUpSpecification signUpSpecification)
    {
        signUpSpecification.AddRuleEmailShouldNotEmpty(this);
        signUpSpecification.AddRuleEmailShouldBeValid(this);
        signUpSpecification.AddRulePasswordShouldNotEmpty(this);
        signUpSpecification.AddRuleNameShouldNotEmpty(this);
        signUpSpecification.AddRulePasswordShouldBeStrong(this);
    }
}