using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soda.Api.Middleware;
using Soda.Application.ApplicationServices.Services.Employers;
using Soda.Application.ApplicationServices.Services.Employers.Payload.Request;
using Soda.Application.ApplicationServices.Services.Employers.Payload.Response;
using Soda.CrossCutting.RequestObjects;
using Soda.CrossCutting.ResultObjects;
using Soda.Domain.Entities;

namespace Soda.Api.Controllers;

/// <summary>
/// Controller for managing employer operations
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[ProducesResponseType(typeof(Result<object>), (int)HttpStatusCode.BadRequest)]
[ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.InternalServerError)]
public class EmployersController(IEmployerService employerService) : ControllerBase
{
    /// <summary>
    /// Gets an employer by ID
    /// </summary>
    /// <param name="id">Employer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The employer details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<Employer>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> GetEmployer([Required] string id, CancellationToken cancellationToken)
    {
        var employer = await employerService.GetByIdAsync(id, cancellationToken);
        if (employer == null)
            return NoContent();

        return Ok(employer);
    }

    /// <summary>
    /// Gets all employers with pagination
    /// </summary>
    /// <param name="request">Pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of employers</returns>
    [HttpGet]
    [ProducesResponseType(typeof(Result<Pagination<Employer>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> GetAllEmployers([FromQuery] PagedRequest request, CancellationToken cancellationToken)
    {
        var result = await employerService.GetAllAsync(request, cancellationToken);
        if (result.Items == null || !result.Items.Any())
            return NoContent();

        return Ok(result);
    }

    /// <summary>
    /// Creates a new employer
    /// </summary>
    /// <param name="input">Employer creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created employer response</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Result<EmployerResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreateEmployer([FromBody] CreateEmployerRequest input, CancellationToken cancellationToken)
    {
        var result = await employerService.CreateAsync(input, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing employer
    /// </summary>
    /// <param name="id">Employer ID</param>
    /// <param name="input">Employer update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated employer response</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<EmployerResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateEmployer([Required] string id, [FromBody] UpdateEmployerRequest input, CancellationToken cancellationToken)
    {
        input.Id = id;
        var result = await employerService.UpdateAsync(input, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes an employer
    /// </summary>
    /// <param name="id">Employer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion response</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<EmployerResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteEmployer([Required] string id, CancellationToken cancellationToken)
    {
        var result = await employerService.DeleteAsync(new DeleteEmployerRequest { Id = id }, cancellationToken);
        return Ok(result);
    }
}