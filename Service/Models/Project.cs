using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service.Models
{
  public class Project
  {
 
    public List<ProjectFile> ProyectFiles { get; set; } = new List<ProjectFile>();
    public string LocalDirectory { get; set; }
    public string RelativePath { get; set; }
    public string ProyectName { get; set; }
    public List<string> ProyectsList { get; set; } = new List<string>();
  }
}
