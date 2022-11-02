using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service.Models
{
  public class Proyect
  {
    public int Id { get; set; }
    public List<ProyectFile> ProyectFiles { get; set; } = new List<ProyectFile>();
    public string LocalDirectory { get; set; }
    public string RelativePath { get; set; }
    public string ProyectName { get; set; }
    public List<string> ProyectsList { get; set; } = new List<string>();
  }
}
