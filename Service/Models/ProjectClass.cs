using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Models
{
  public class ProjectClass
  {
    public string Name { get; set; }
    public int Count { get; set; }
    public List<ProjectFunction> ProyectFunctions { get; set; } = new List<ProjectFunction>();
  }
}
