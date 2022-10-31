using NUnit.Framework;
using Service;
using Service.Models;

namespace TestProjectX
{
  public class Tests
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
      ReportService reportService = new ReportService();

      var proyect = new Proyect()
      {
        LocalDirectory= "X:\\X_XP_Viewer",
        RelativePath = "\\Wigos System\\WGC\\GUI",
        ProyectName = "GUI Test"
      };

      proyect.ProyectFiles.Add(new ProyectFile
      {
        Namespace = "WSI.Common",
         ProyectClasses = { new ProyectClass { 
          Name = "BucketsUpdate",
           ProyectFunctions = { new ProyectFunction { FunctionName = "UpdateCustomerBucket" } }
         } }

      });



      reportService.BuildReport(proyect);


      
    }
  }
}