using Carter;
using MediatR;
using ReportService.Application.Features.Reports.Commands.RequestReport;
using ReportService.Application.Features.Reports.DTOs;
using ReportService.Application.Features.Reports.Queries.GetReportDetails;
using ReportService.Application.Features.Reports.Queries.ListReports;

namespace ReportService.Api.Modules
{
  public class ReportModule : ICarterModule
  {
    public void AddRoutes(IEndpointRouteBuilder app)
    {
      var group = app.MapGroup("/api/reports")
                     .WithTags("Reports");


      group.MapPost("/", async (IMediator mediator) =>
      {
        var command = new RequestReportCommand(); 
        var reportId = await mediator.Send(command);


        return Results.Accepted($"/api/reports/{reportId}", new { Id = reportId });
      })
      .WithName("RequestReport")
      .Produces(StatusCodes.Status202Accepted);



      group.MapGet("/", async (IMediator mediator) =>
      {
        var query = new ListReportsQuery();
        var result = await mediator.Send(query);
        return Results.Ok(result);
      })
      .WithName("ListReports")
      .Produces<List<ReportSummaryDto>>(StatusCodes.Status200OK);


      group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
      {
        var query = new GetReportDetailsQuery(id);
        var result = await mediator.Send(query);
        return result is not null ? Results.Ok(result) : Results.NotFound();
      })
               .WithName("GetReportDetails")
               .Produces<ReportDetailDto>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound);
    }
  }
}
