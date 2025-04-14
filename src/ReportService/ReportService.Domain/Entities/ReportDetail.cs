using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Domain.Entities
{
  public class ReportDetail
  {
    public Guid Id { get; set; }
    public Guid ReportId { get; set; } 
    public string KonumBilgisi { get; set; } = string.Empty;
    public int KisiSayisi { get; set; }
    public int TelefonNumarasiSayisi { get; set; }

 
    public virtual Report Report { get; set; } = null!; 
  }
}
