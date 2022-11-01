using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Models;
using System.IO;
using System.Text;

namespace ProyectoX.Controllers.Api
{
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class ReportController : ControllerBase
  {
    ReportService reportService = new ReportService();

    Proyect proyecto2;

    string pathHtml = "X:\\Git\\tmpx\\ProyectoX\\wwwroot\\html\\";

    string pathDestinationHtml = "X:\\Git\\report\\";

    public ReportController()
    {
      proyecto2 = reportService.GenerateProyect("X:\\FGNUGETUPDATE\\Wigos System\\WGC\\Kernel\\WSI.CommonA\\Buckets.cs");
      proyecto2.LocalDirectory = "X:\\FGNUGETUPDATE";
      proyecto2.RelativePath = "\\Wigos System"; //WGC\\GUI
      proyecto2.ProyectName = "GUI Test";
    }



    [HttpGet]
    public Report Build()
    {
      var response = reportService.BuildReport(proyecto2);

    reportService.WriteReportHtml(response,"report", pathHtml, pathDestinationHtml);

      return response;

      

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
     

      var response = reportService.BuildReport2(proyecto2);

     reportService.WriteReportHtml(response, "report", pathHtml, pathDestinationHtml);

      return response;

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
