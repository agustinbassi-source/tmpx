using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Models
{
  public class Report
  {
    public List<string> Paths { get; set; } = new List<string>();
    public List<string> UniquePaths { get; set; } = new List<string>();
    public Dictionary<string, int> UniquePathsCount { get; set; } = new Dictionary<string, int>();
    public List<ReportItem> Items { get; set; } = new List<ReportItem>();
  }

  public class ReportItem
  {
    public List<ReportItem> Items { get; set; } = new List<ReportItem>();
    public string  Data { get; set; }
    public string DataRaw { get; set; }
    public string Path { get; set; }
    public string Code { get; set; }
    public int LineNumber { get; set; }
    public int FilesCount { get; set; }
    public string FilesFindText { get; set; }
    public List<string> FilesFind { get; set; } = new List<string>();
    public string LineCode { get; set; }




  }

}
