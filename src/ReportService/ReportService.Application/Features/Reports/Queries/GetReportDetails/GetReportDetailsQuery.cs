using MediatR;
using ReportService.Application.Features.Reports.DTOs;

namespace ReportService.Application.Features.Reports.Queries.GetReportDetails
{
  public class GetReportDetailsQuery : IRequest<ReportDetailDto?>
  {
    public Guid Id { get; set; }

    public GetReportDetailsQuery(Guid id)
    {
      Id = id;
    }
  }
}
