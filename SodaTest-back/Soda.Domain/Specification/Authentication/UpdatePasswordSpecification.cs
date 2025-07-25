using FluentValidation;
using Soda.Domain.Models.Authentication.UpdatePassword;

namespace Soda.Domain.Specification.Authentication;
public interface IUpdatePasswordSpecification
{
    void AddRuleIdShouldNotEmpty(AbstractValidator<UpdatePasswordModel> validator);
    void AddRulePasswordShouldNotEmpty(AbstractValidator<UpdatePasswordModel> validator);
    void AddRulePasswordShouldBeStrong(AbstractValidator<UpdatePasswordModel> validator);
}

public class UpdatePasswordSpecification : IUpdatePasswordSpecification
{
    public void AddRuleIdShouldNotEmpty(AbstractValidator<UpdatePasswordModel> validator)
    {
        validator.RuleFor(entity => entity.Id)
            .NotEmpty().WithMessage("Id is required");
    }

    public void AddRulePasswordShouldNotEmpty(AbstractValidator<UpdatePasswordModel> validator)
    {
        validator.RuleFor(entity => entity.Password)
            .NotEmpty().WithMessage("Password is required");
    }

    public void AddRulePasswordShouldBeStrong(AbstractValidator<UpdatePasswordModel> validator)
    {
        validator.RuleFor(entity => entity.Password)
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter") 
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
    }
}