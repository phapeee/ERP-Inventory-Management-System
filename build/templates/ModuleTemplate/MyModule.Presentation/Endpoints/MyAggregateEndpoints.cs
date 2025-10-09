using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MyModule.Application.Commands.CreateMyAggregate;
using MyModule.Application.Queries.GetMyAggregateById;
using MyModule.Application.Dtos;

namespace MyModule.Presentation.Endpoints;

internal static class MyAggregateEndpoints
{
    public static void MapMyAggregateEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/my-aggregates").WithTags("MyAggregates");

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var query = new GetMyAggregateByIdQuery(id);
            var result = await sender.Send(query);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetMyAggregateById")
        .Produces<MyAggregateDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateMyAggregateCommand command, ISender sender) =>
        {
            var id = await sender.Send(command);
            return Results.CreatedAtRoute("GetMyAggregateById", new { id }, id);
        })
        .WithName("CreateMyAggregate")
        .Produces<Guid>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
