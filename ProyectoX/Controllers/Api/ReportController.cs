using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Models;

namespace ProyectoX.Controllers.Api
{
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class ReportController : ControllerBase
  {
    ReportService reportService = new ReportService();

    Proyect proyecto2;

    public ReportController()
    {
      proyecto2 = reportService.GenerateProyect("X:\\FGNUGETUPDATE\\Wigos System\\WGC\\Kernel\\WSI.CommonA\\Buckets.cs");
      proyecto2.LocalDirectory = "X:\\FGNUGETUPDATE";
      proyecto2.RelativePath = "\\Wigos System\\WGC\\GUI"; //WGC\\GUI
      proyecto2.ProyectName = "GUI Test";
    }



    [HttpGet]
    public Report Build()
    {
      return reportService.BuildReport(proyecto2);

      // ReportService reportService = new ReportService();

      //var proyecto2 = reportService.GenerateProyect("X:\\FGNUGETUPDATE\\Wigos System\\WGC\\Kernel\\WSI.CommonA\\Buckets.cs");

      //proyecto2.LocalDirectory = "C:\\C_Source_Main";
      //proyecto2.RelativePath = "\\Wigos System\\"; //WGC\\GUI
      //proyecto2.ProyectName = "GUI Test";

   

      //var proyect = new Proyect()
      //{
      //  LocalDirectory = "C:\\C_Source_Main",
      //  RelativePath = "\\Wigos System\\WGC\\GUI",
      //  ProyectName = "GUI Test"
      //};

      //proyect.ProyectFiles.Add(new ProyectFile
      //{
      //  Namespace = "WSI.Common",
      //  ProyectClasses = {


      //    new ProyectClass {
      //    Name = "BucketsUpdate",
      //     ProyectFunctions = {
      //      new ProyectFunction { FunctionName = "UpdateCustomerBucket" }
      //     , new ProyectFunction { FunctionName= "FromBucketDictToTable" }
      //       , new ProyectFunction { FunctionName= "BatchUpdateBuckets" } }
      //   },


      //    //new ProyectClass{ Name = "Buckets", ProyectFunctions = { 
      //    //    new ProyectFunction { FunctionName= "BatchBucketsEmpty" },
      //    //   new ProyectFunction { FunctionName = "BucketsHaveSameConfiguration" } } }  }
      //  }
      //}

      //  );


    }

    [HttpGet]
    public Report BuildDos()
    {
     

      return reportService.BuildReport2(proyecto2);

      //var proyect = new Proyect()
      //{
      //  LocalDirectory = "C:\\C_Source_Main",
      //  RelativePath = "\\Wigos System\\WGC\\GUI",
      //  ProyectName = "GUI Test"
      //};

      //proyect.ProyectFiles.Add(new ProyectFile
      //{
      //  Namespace = "WSI.Common",
      //  ProyectClasses = {


      //    new ProyectClass {
      //    Name = "BucketsUpdate",
      //     ProyectFunctions = {
      //      new ProyectFunction { FunctionName = "UpdateCustomerBucket" }
      //     , new ProyectFunction { FunctionName= "FromBucketDictToTable" }
      //       , new ProyectFunction { FunctionName= "BatchUpdateBuckets" } }
      //   },


      //    //new ProyectClass{ Name = "Buckets", ProyectFunctions = { 
      //    //    new ProyectFunction { FunctionName= "BatchBucketsEmpty" },
      //    //   new ProyectFunction { FunctionName = "BucketsHaveSameConfiguration" } } }  }
      //  }
      //}

      //  );


    }
  }
}
