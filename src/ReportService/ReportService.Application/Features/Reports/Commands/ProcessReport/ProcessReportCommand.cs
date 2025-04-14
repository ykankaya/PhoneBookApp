using MediatR;

namespace ReportService.Application.Features.Reports.Commands.ProcessReport
{
  public class ProcessReportCommand : IRequest<Unit>
  {
    public Guid ReportId { get; set; }

    public ProcessReportCommand(Guid reportId)
    {
      ReportId = reportId;
    }
  }
}
