using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Models
{
  public class ProjectFile
  {
    public int Id { get; set; }
    public int ProyectId { get; set; }
    public string Namespace { get; set; }
    public string Path { get; set; }
    public List<ProjectClass> ProyectClasses { get; set; } = new List<ProjectClass>();
  }
}
