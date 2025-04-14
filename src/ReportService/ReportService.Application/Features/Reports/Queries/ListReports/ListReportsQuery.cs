using MediatR;
using ReportService.Application.Features.Reports.DTOs;

namespace ReportService.Application.Features.Reports.Queries.ListReports
{
  public class ListReportsQuery : IRequest<List<ReportSummaryDto>>
  {
  }
}
