using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using PineConePro.Erp.MyModule.Application.Dtos;
using PineConePro.Erp.MyModule.Application.Queries.GetMyAggregateById;
using PineConePro.Erp.MyModule.Domain.Aggregates;
using PineConePro.Erp.MyModule.Domain.Interfaces;

namespace PineConePro.Erp.MyModule.Application.Tests.Queries.GetMyAggregateById;

/// <summary>
/// Unit tests for the <see cref="GetMyAggregateByIdQueryHandler"/> class.
/// </summary>
public sealed class GetMyAggregateByIdQueryHandlerTests
{
    private readonly IMyAggregateRepository _repository = Substitute.For<IMyAggregateRepository>();
    private readonly GetMyAggregateByIdQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMyAggregateByIdQueryHandlerTests"/> class.
    /// </summary>
    public GetMyAggregateByIdQueryHandlerTests()
    {
        _handler = new GetMyAggregateByIdQueryHandler(_repository);
    }

    /// <summary>
    /// Tests that the Handle method returns an aggregate DTO when the aggregate exists.
    /// </summary>
    [Fact]
    public async Task HandleShouldReturnAggregateDtoWhenAggregateExists()
    {
        // Arrange
        var aggregate = MyAggregate.Create("Test Aggregate");
        _repository.GetByIdAsync(aggregate.Id, CancellationToken.None).Returns(aggregate);
        var query = new GetMyAggregateByIdQuery(aggregate.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Should().BeOfType<MyAggregateDto>();
        result!.Id.Should().Be(aggregate.Id);
        result!.Name.Should().Be(aggregate.Name);
    }

    /// <summary>
    /// Tests that the Handle method returns null when the aggregate does not exist.
    /// </summary>
    [Fact]
    public async Task HandleShouldReturnNullWhenAggregateDoesNotExist()
    {
        // Arrange
        _repository.GetByIdAsync(Arg.Any<Guid>(), CancellationToken.None).Returns(Task.FromResult<MyAggregate?>(null));
        var query = new GetMyAggregateByIdQuery(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
