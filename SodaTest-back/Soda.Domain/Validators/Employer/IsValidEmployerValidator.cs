using FluentValidation;
using Soda.Domain.Specification;

namespace Soda.Domain.Validators;

public interface IIsValidEmployerValidator : IValidator<Entities.Employer>;

public class IsValidEmployerValidator : AbstractValidator<Entities.Employer>, IIsValidEmployerValidator
{
    public IsValidEmployerValidator(ICreateEmployerSpecification specification)
    {
        specification.AddRuleTaxDocumentShouldNotEmpty(this);
        specification.AddRuleTaxDocumentShouldBeUnique(this);
        specification.AddRuleFirstNameShouldNotEmpty(this);
        specification.AddRuleLastNameShouldNotEmpty(this);
        specification.AddRuleBirthDayShouldNotEmpty(this);
        specification.AddRuleBirthDayShouldValid(this);
        specification.AddRuleTypeLevelShouldBeLessOrEqual(this);
        specification.AddRuleShouldBeMajority(this);
        specification.AddRuleExternalIdShouldNotEmpty(this);
    }
}