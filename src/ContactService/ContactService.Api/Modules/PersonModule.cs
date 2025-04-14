using Carter;
using ContactService.Application.Features.ContactInfos.Commands.AddContactInfo;
using ContactService.Application.Features.ContactInfos.Commands.RemoveContactInfo;
using ContactService.Application.Features.Persons.Commands.CreatePerson;
using ContactService.Application.Features.Persons.Commands.DeletePerson;
using ContactService.Application.Features.Persons.DTOs;
using ContactService.Application.Features.Persons.Queries.GetAllPersons;
using ContactService.Application.Features.Persons.Queries.GetPersonById;
using MediatR;

namespace ContactService.Api.Modules
{
    public class PersonModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/persons")
                .WithTags("Persons");

            group.MapGet("/", async (IMediator mediator) =>
                {
                    var query = new GetAllPersonsQuery();
                    var result = await mediator.Send(query);
                    return Results.Ok(result);
                })
                .WithName("GetAllPersons")
                .Produces<List<PersonDto>>(StatusCodes.Status200OK);

            group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
                {
                    var query = new GetPersonByIdQuery(id);
                    var result = await mediator.Send(query);
                    return result is not null ? Results.Ok(result) : Results.NotFound();
                })
                .WithName("GetPersonById")
                .Produces<PersonDetailDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPost("/", async (CreatePersonCommand command, IMediator mediator) =>
                {
                    var personId = await mediator.Send(command);

                    return Results.CreatedAtRoute("GetPersonById", new { id = personId }, new { Id = personId });
                })
                .WithName("CreatePerson")
                .Produces(StatusCodes.Status201Created)
                .ProducesValidationProblem();

            group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator) =>
                {
                    var command = new DeletePersonCommand(id);
                    var success = await mediator.Send(command);
                    return success ? Results.NoContent() : Results.NotFound();
                })
                .WithName("DeletePerson")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPost("/{id:guid}/contacts",
                    async (Guid id, AddContactInfoCommand partialCommand, IMediator mediator) =>
                    {
                        partialCommand.PersonId = id; // Route'dan gelen id'yi komuta ata
                        var contactInfoId = await mediator.Send(partialCommand);
                        return Results.Ok(new { Id = contactInfoId });
                    })
                .WithName("AddContactInfo")
                .Produces(StatusCodes.Status200OK)
                .ProducesValidationProblem();


            group.MapDelete("/{personId:guid}/contacts/{contactId:guid}",
                    async (Guid personId, Guid contactId, IMediator mediator) =>
                    {
                        var command = new RemoveContactInfoCommand(personId, contactId);
                        var success = await mediator.Send(command);
                        return success ? Results.NoContent() : Results.NotFound();
                    })
                .WithName("RemoveContactInfo")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
        }
    }
}