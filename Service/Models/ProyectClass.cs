using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Models
{
  public class ProyectClass
  {
    public string Name { get; set; }
    public int Count { get; set; }
    public List<ProyectFunction> ProyectFunctions { get; set; } = new List<ProyectFunction>();
  }
}
