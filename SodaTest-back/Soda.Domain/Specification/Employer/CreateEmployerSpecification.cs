using FluentValidation;
using Soda.Domain.DomainServices.Users;
using Soda.Domain.Entities;
using Soda.Domain.Repositories;


namespace Soda.Domain.Specification;
public interface ICreateEmployerSpecification
{
    void AddRuleTaxDocumentShouldNotEmpty(AbstractValidator<Entities.Employer> validator);
    void AddRuleTaxDocumentShouldBeUnique(AbstractValidator<Entities.Employer> validator);
    void AddRuleFirstNameShouldNotEmpty(AbstractValidator<Entities.Employer> validator);
    void AddRuleLastNameShouldNotEmpty(AbstractValidator<Entities.Employer> validator);
    void AddRuleExternalIdShouldNotEmpty(AbstractValidator<Entities.Employer> validator);
    void AddRuleBirthDayShouldNotEmpty(AbstractValidator<Entities.Employer> validator);
    void AddRuleBirthDayShouldValid(AbstractValidator<Entities.Employer> validator);
    Task AddRuleTypeLevelShouldBeLessOrEqual(AbstractValidator<Employer> validator);
    void AddRuleShouldBeMajority(AbstractValidator<Entities.Employer> validator);
}
public class CreateEmployerSpecification(IEmployerRepository repository, UserServiceContext userServiceContext) : ICreateEmployerSpecification
{
    public void AddRuleTaxDocumentShouldNotEmpty(AbstractValidator<Employer> validator)
    {
        validator.RuleFor(entity => entity.TaxDocument)
            .NotEmpty().WithMessage("Tax document is required");
    }

    public void AddRuleTaxDocumentShouldBeUnique(AbstractValidator<Employer> validator)
    {
        validator.RuleFor(entity => entity.TaxDocument)
            .MustAsync(async (taxDocument, cancellationToken) =>
            {
                var entity = await repository.GetByTaxDocumentAsync(taxDocument, cancellationToken);
                return entity == null;
            })
            .WithMessage("Tax document must be unique");
    }

    public void AddRuleFirstNameShouldNotEmpty(AbstractValidator<Employer> validator)
    {
        validator.RuleFor(entity => entity.FirstName)
            .NotEmpty().WithMessage("First name is required");
    }

    public void AddRuleLastNameShouldNotEmpty(AbstractValidator<Employer> validator)
    {
        validator.RuleFor(entity => entity.LastName)
            .NotEmpty().WithMessage("Last name is required");
    }

    public void AddRuleExternalIdShouldNotEmpty(AbstractValidator<Employer> validator)
    {
        validator.RuleFor(entity => entity.ExternalId)
            .NotEmpty().WithMessage("External ID is required");
    }

    public void AddRuleBirthDayShouldNotEmpty(AbstractValidator<Employer> validator)
    {
        validator.RuleFor(entity => entity.BirthDate)
            .NotEmpty().WithMessage("Birth date is required");
    }

    public void AddRuleBirthDayShouldValid(AbstractValidator<Employer> validator)
    {
        validator.RuleFor(entity => entity.BirthDate)
            .GreaterThan(DateOnly.MinValue).LessThan(DateOnly.MaxValue).WithMessage("Birth date must be valid");
    }

    public Task AddRuleTypeLevelShouldBeLessOrEqual(AbstractValidator<Employer> validator)
    {
        validator.RuleFor(entity => entity.Type)
            .MustAsync(async (newType, cancellationToken) =>
            {

                var currentEmployer = await repository.GetByExternalIdAsync(userServiceContext.UserAuth.Id, cancellationToken);
                return newType <= currentEmployer!.Type;
            })
            .WithMessage("Type level must be less than or equal to current employer's type");
        return Task.CompletedTask;
    }

    public void AddRuleShouldBeMajority(AbstractValidator<Employer> validator)
    {
        validator.RuleFor(entity => entity.Age)
            .GreaterThanOrEqualTo(18).WithMessage("Age must be 18 or greater");
    }
}