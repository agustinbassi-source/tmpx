using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Models
{
  public class Report
  {
    public List<ReportItem> Items { get; set; } = new List<ReportItem>();
  }

  public class ReportItem
  {
    public List<ReportItem> Items { get; set; } = new List<ReportItem>();
    public string  Data { get; set; }
    public string Path { get; set; }
    public string Code { get; set; }
    

  }

}
