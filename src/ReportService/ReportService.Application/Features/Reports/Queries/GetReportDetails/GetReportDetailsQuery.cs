using MediatR;
using ReportService.Application.Features.Reports.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
