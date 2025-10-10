using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PineConePro.Erp.MyModule.Domain.Aggregates;

namespace PineConePro.Erp.MyModule.Domain.Interfaces;

/// <summary>
/// Defines the repository for the MyAggregate aggregate.
/// </summary>
public interface IMyAggregateRepository
{
    /// <summary>
    /// Gets an aggregate by its identifier.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The aggregate.</returns>
    Task<MyAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all aggregates.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The aggregates.</returns>
    Task<IReadOnlyCollection<MyAggregate>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an aggregate.
    /// </summary>
    /// <param name="aggregate">The aggregate to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(MyAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an aggregate.
    /// </summary>
    /// <param name="aggregate">The aggregate to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(MyAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an aggregate.
    /// </summary>
    /// <param name="aggregate">The aggregate to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(MyAggregate aggregate, CancellationToken cancellationToken = default);
}
