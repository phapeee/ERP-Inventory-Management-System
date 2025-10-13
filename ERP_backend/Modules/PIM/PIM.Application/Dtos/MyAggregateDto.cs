namespace PineConePro.Erp.PIM.Application.Dtos;

/// <summary>
/// Represents the data transfer object for MyAggregate.
/// </summary>
/// <param name="Id">The identifier of the aggregate.</param>
/// <param name="Name">The name of the aggregate.</param>
public sealed record MyAggregateDto(Guid Id, string Name);
