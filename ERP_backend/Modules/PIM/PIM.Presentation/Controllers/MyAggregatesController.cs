using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PineConePro.Erp.PIM.Application.Commands.CreateMyAggregate;
using PineConePro.Erp.PIM.Application.Commands.DeleteMyAggregate;
using PineConePro.Erp.PIM.Application.Commands.UpdateMyAggregate;
using PineConePro.Erp.PIM.Application.Dtos;
using PineConePro.Erp.PIM.Application.Queries.GetAllMyAggregates;
using PineConePro.Erp.PIM.Application.Queries.GetMyAggregateById;

namespace PineConePro.Erp.PIM.Presentation.Controllers;

/// <summary>
/// RESTful controller for managing MyAggregate resources.
/// </summary>
[ApiController]
[Route("api/my-aggregates")]
[Produces("application/json")]
public sealed class MyAggregatesController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyAggregatesController"/> class.
    /// </summary>
    /// <param name="sender">Mediator abstraction used to dispatch commands and queries.</param>
    public MyAggregatesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets all aggregates.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<MyAggregateDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<MyAggregateDto>>> GetAsync(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllMyAggregatesQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific aggregate by identifier.
    /// </summary>
    [HttpGet("{id:guid}", Name = "GetMyAggregateById")]
    [ProducesResponseType(typeof(MyAggregateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MyAggregateDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMyAggregateByIdQuery(id), cancellationToken);
        return result is not null ? Ok(result) : NotFound();
    }

    /// <summary>
    /// Creates a new aggregate.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MyAggregateDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MyAggregateDto>> CreateAsync(CreateMyAggregateCommand command, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);
        var createdAggregate = await _sender.Send(new GetMyAggregateByIdQuery(id), cancellationToken)
                                 ?? new MyAggregateDto(id, command.Name.Trim());

        return CreatedAtRoute("GetMyAggregateById", new { id }, createdAggregate);
    }

    /// <summary>
    /// Updates an existing aggregate.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateMyAggregateCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            ModelState.AddModelError(nameof(command.Id), "Body identifier must match the route identifier.");
            return ValidationProblem(ModelState);
        }

        var updateSucceeded = await _sender.Send(command, cancellationToken);
        return updateSucceeded ? NoContent() : NotFound();
    }

    /// <summary>
    /// Deletes an aggregate.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleteSucceeded = await _sender.Send(new DeleteMyAggregateCommand(id), cancellationToken);
        return deleteSucceeded ? NoContent() : NotFound();
    }
}
