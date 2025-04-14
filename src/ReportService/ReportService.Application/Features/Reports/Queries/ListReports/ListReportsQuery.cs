using MediatR;
using ReportService.Application.Features.Reports.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Application.Features.Reports.Queries.ListReports
{
  public class ListReportsQuery : IRequest<List<ReportSummaryDto>>
  {
  }
}
