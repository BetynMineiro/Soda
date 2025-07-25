using Auth0.ManagementApi.Models;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Soda.Application.ApplicationServices.NotificationService;
using Soda.Application.ApplicationServices.Services.Employers;
using Soda.Application.ApplicationServices.Services.Employers.Payload.Request;
using Soda.Domain.DomainServices.Authentication.Interfaces.Authentication;
using Soda.Domain.Entities;
using Soda.Domain.Validators;
using Soda.Domain.Validators.Authentication;
using Soda.Domain.Validators.Employer;
using Soda.Postgres.EF.Adapter.Repository.Base;
using Soda.Postgres.EF.Adapter.UnitOfWork;

namespace TestProject;

public class EmployerServiceTests
{
    private readonly Mock<ILogger<EmployerService>> _loggerMock;
    private readonly Mock<IIsValidEmployerValidator> _employerValidatorMock;
    private readonly Mock<IIsValidSignUpValidator> _signUpValidatorMock;
    private readonly Mock<NotificationServiceContext> _notificationServiceMock;
    private readonly Mock<IAuthenticationService> _authServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPostgresRepositoryBase<Employer>> _repositoryMock;
    private readonly EmployerService _service;

    public EmployerServiceTests()
    {
        _loggerMock = new Mock<ILogger<EmployerService>>();
        _employerValidatorMock = new Mock<IIsValidEmployerValidator>();
        _signUpValidatorMock = new Mock<IIsValidSignUpValidator>();
        _notificationServiceMock = new Mock<NotificationServiceContext>();
        _authServiceMock = new Mock<IAuthenticationService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _repositoryMock = new Mock<IPostgresRepositoryBase<Employer>>();

        _unitOfWorkMock
            .Setup(x => x.Repository<Employer>())
            .Returns(_repositoryMock.Object);

        _service = new EmployerService(
            _loggerMock.Object,
            _employerValidatorMock.Object,
            _signUpValidatorMock.Object,
            _notificationServiceMock.Object,
            _authServiceMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task CreateAsync_WhenValidRequest_ShouldCreateEmployer()
    {
        // Arrange
        var request = new CreateEmployerRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "password123",
            Type = EmployerType.Employee,
            BirthDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
            TaxDocument = "123456789"
        };

        _employerValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<Employer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _signUpValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<SignUpModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _authServiceMock
            .Setup(x => x.SingUpAsync(It.IsAny<SignUpModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("external-id-123");
        
        _authServiceMock
            .Setup(x => x.GetUserInfoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Picture = "avatar-url" });

        // Act
        var result = await _service.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.InsertAsync(It.IsAny<Employer>(), It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task UpdateAsync_WhenEmployerExists_ShouldUpdateEmployer()
    {
        // Arrange
        var employerId = Guid.NewGuid();
        var request = new UpdateEmployerRequest
        {
            Id = employerId.ToString(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Type = EmployerType.Employee
        };

        var existingEmployer = new Employer { Id = employerId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(employerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEmployer);

        _employerValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<Employer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await _service.UpdateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Employer>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteEmployer()
    {
        // Arrange
        var employerId = Guid.NewGuid();
        var request = new DeleteEmployerRequest { Id = employerId.ToString() };

        // Act
        var result = await _service.DeleteAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.DeleteAsync(employerId, It.IsAny<CancellationToken>()), Times.Once);
    }
}