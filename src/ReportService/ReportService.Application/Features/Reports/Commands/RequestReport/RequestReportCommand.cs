using MediatR;

namespace ReportService.Application.Features.Reports.Commands.RequestReport
{
  public class RequestReportCommand : IRequest<Guid>
  {
  }
}
