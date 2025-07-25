using FluentValidation;
using Soda.Domain.Models.Authentication.UpdatePassword;
using Soda.Domain.Specification.Authentication;

namespace Soda.Domain.Validators.Authentication;

public interface IIsValidUpdatePasswordValidator : IValidator<UpdatePasswordModel>;
public class IsValidUpdatePasswordValidator : AbstractValidator<UpdatePasswordModel>,  IIsValidUpdatePasswordValidator
{
    public IsValidUpdatePasswordValidator(IUpdatePasswordSpecification updatePasswordSpecification)
    {
        updatePasswordSpecification.AddRulePasswordShouldNotEmpty(this);
        updatePasswordSpecification.AddRulePasswordShouldBeStrong(this);
        updatePasswordSpecification.AddRuleIdShouldNotEmpty(this);
        
    }
}