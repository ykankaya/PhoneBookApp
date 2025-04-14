using ReportService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Application.Features.Reports.DTOs
{
  public class ReportSummaryDto
  {
    public Guid Id { get; set; }
    public DateTime TalepEdildigiTarih { get; set; }
    public ReportStatus Durum { get; set; }
  }
}
