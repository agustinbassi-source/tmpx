using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Models
{
  public class ProyectFile
  {
    public int Id { get; set; }
    public int ProyectId { get; set; }
    public string Namespace { get; set; }
    public string Path { get; set; }
    public List<ProyectClass> ProyectClasses { get; set; } = new List<ProyectClass>();
  }
}
