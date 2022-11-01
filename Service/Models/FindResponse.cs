using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Models
{
  public class FindResponse
  {
    public System.IO.FileInfo File { get; set; }
    public int LineNumber { get; set; }
    public string LineCode { get; set; }

  }
}
