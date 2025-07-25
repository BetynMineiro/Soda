using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Soda.Application.ApplicationServices.NotificationService;
using Soda.Application.ApplicationServices.Services.Employers.Payload.Request;
using Soda.Application.ApplicationServices.Services.Employers.Payload.Response;
using Soda.CrossCutting.RequestObjects;
using Soda.CrossCutting.ResultObjects;
using Soda.Domain.DomainServices.Authentication.Interfaces.Authentication;
using Soda.Domain.Entities;
using Soda.Domain.Validators;
using Soda.Domain.Validators.Authentication;
using Soda.Postgres.EF.Adapter.UnitOfWork;

namespace Soda.Application.ApplicationServices.Services.Employers;

public class EmployerService(
    ILogger<EmployerService> logger,
    IIsValidEmployerValidator employerValidator,
    IIsValidSignUpValidator isValidSignUpValidator,
    NotificationServiceContext notificationServiceContext,
    IAuthenticationService authenticationService,
    IUnitOfWork unitOfWork)
    : IEmployerService
{
    public async Task<EmployerResponse?> CreateAsync(CreateEmployerRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting employee creation: {FirstName} {LastName}", request.FirstName, request.LastName);

        try
        {
            var employer = new Employer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Type = request.Type,
                Email = request.Email,
                TaxDocument = request.TaxDocument,
                BirthDate = request.BirthDate,
                PhoneNumbers = request.PhoneNumbers,
                ManagerId =  string.IsNullOrWhiteSpace(request.ManagerId)? null:  Guid.Parse(request.ManagerId)
            };
            
            var result = await employerValidator.ValidateAsync(employer, cancellationToken);
            if (!result.IsValid)
            {
                logger.LogWarning("Employee validation failed | Errors: {errors}", string.Join(", ", result.Errors));
                notificationServiceContext.AddNotifications(result);
                return null;
            }

            var signUpInput = new SignUpModel()
            {
                Name = request.FirstName + " " + request.LastName,
                Email = request.Email,
                Password = request.Password
            };

            var resultSingUp = await isValidSignUpValidator.ValidateAsync(signUpInput, cancellationToken);
            if (!resultSingUp.IsValid)
            {
                logger.LogWarning("SignUp validation failed | Errors: {errors}", string.Join(", ", result.Errors));
                notificationServiceContext.AddNotifications(result);
                return null;
            }
            
            logger.LogInformation("SignUp validation succeeded");

            var created = await authenticationService.SingUpAsync(signUpInput, cancellationToken);
            if (string.IsNullOrEmpty(created))
            {
                logger.LogWarning("Signup failed | Reason: Created ID is null or empty");
                notificationServiceContext.AddNotification("Signup Fail");
                return null;
            }
            logger.LogInformation("User signed up | CreatedId: {createdId}", created);

            var userAuth0 = await authenticationService.GetUserInfoAsync(created, cancellationToken);

            employer.ExternalId = created;
            employer.Avatar = userAuth0.Picture;
            
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            var repo = unitOfWork.Repository<Employer>();
            await repo.InsertAsync(employer, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            logger.LogInformation("Employee created successfully (Id: {Id})", employer.Id);
            return new EmployerResponse("Employee created successfully.", employer.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating employee: {Message}", ex.Message);
            await unitOfWork.RollbackAsync(cancellationToken);
            return null;
        }
    }

    public async Task<EmployerResponse?> UpdateAsync(UpdateEmployerRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting employee update (Id: {Id})", request.Id);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            var repo = unitOfWork.Repository<Employer>();

            var existing = await repo.GetByIdAsync(new Guid(request.Id), cancellationToken);
            if (existing == null)
            {
                logger.LogWarning("Employee not found (Id: {Id})", request.Id);
                await unitOfWork.RollbackAsync(cancellationToken);
                notificationServiceContext.AddNotification($"Employee not found (Id: {request.Id})");
                return null;
            }

            existing.FirstName = request.FirstName;
            existing.LastName = request.LastName;
            existing.Email = request.Email;
            existing.BirthDate = request.BirthDate;
            existing.PhoneNumbers = request.PhoneNumbers;
            existing.TaxDocument = request.TaxDocument;
            existing.Type = request.Type;
            existing.ManagerId = string.IsNullOrWhiteSpace(request.ManagerId) ? null : Guid.Parse(request.ManagerId);
            existing.Avatar = request.Avatar;
            existing.SetUpdateInfo();
            
            var result = await employerValidator.ValidateAsync(existing, cancellationToken);
            if (!result.IsValid)
            {
                logger.LogWarning("Employee validation failed | Errors: {errors}", string.Join(", ", result.Errors));
                notificationServiceContext.AddNotifications(result);
                return null;
            }
            
            await repo.UpdateAsync(existing, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            logger.LogInformation("Employee updated successfully (Id: {Id})", request.Id);
            return new EmployerResponse("Employee updated successfully.", new Guid(request.Id));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating employee: {Message}", ex.Message);
            await unitOfWork.RollbackAsync(cancellationToken);
            return null;
        }
    }

    public async Task<EmployerResponse?> DeleteAsync(DeleteEmployerRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting employee deletion (Id: {Id})", request.Id);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            var repo = unitOfWork.Repository<Employer>();

            await repo.DeleteAsync(new Guid(request.Id), cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            logger.LogInformation("Employee deleted successfully (Id: {Id})", request.Id);

            return new EmployerResponse("Employee deleted successfully.", new Guid(request.Id));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting employee: {Message}", ex.Message);
            await unitOfWork.RollbackAsync(cancellationToken);
            return null;
        }
    }

    public async Task<Employer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.Repository<Employer>();
        return await repo.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Pagination<Employer>> GetAllAsync(PagedRequest request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.Repository<Employer>();
        var query = repo.GetAllAsync();
        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new Pagination<Employer>(items, totalItems, request.PageNumber, request.PageSize);
    }
}