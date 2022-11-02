using Service.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Service
{
  public class ReportService
  {
    private readonly SearchService searchService = new SearchService();

    public Report BuildReport3(Proyect proyect)
    {
      var reportInitial = BuildReport(proyect);

      Report response = new Report();

      response.Paths = reportInitial.Paths.OrderBy(x => x).ToList();

      response.UniquePaths = reportInitial.UniquePaths.OrderBy(x => x).ToList();

      response.UniquePathsCount = reportInitial.UniquePathsCount;

      var file = proyect.ProyectFiles[0];

    

      return response;
    }


    public Report BuildReport2(Proyect proyect)
    {
      var reportInitial = BuildReport(proyect);

      Report response = new Report();

      response.Paths = reportInitial.Paths.OrderBy(x=> x).ToList();

      response.UniquePaths = reportInitial.UniquePaths.OrderBy(x => x).ToList();

      response.UniquePathsCount = reportInitial.UniquePathsCount;

      var file = proyect.ProyectFiles[0];

      foreach (var classx in file.ProyectClasses)
      {
        ReportItem classxItem = new ReportItem();

        ReportItem classReport = new ReportItem();

        foreach (var item in reportInitial.Items)
        {
          var tmpGet = item.Items.Where(x => x.Data == classx.Name).FirstOrDefault();

          if (tmpGet != null)
          {
            classReport = tmpGet;
          }
        }

        //    var classesReport =   reportInitial.Items.Where(x => x.Data == classx.Name).ToList();

        classxItem.Data = classx.Name;

        classxItem.DataRaw = classx.Name;

        classxItem.FilesFind = classReport.FilesFind;

        classxItem.Data += " (total: " + classx.Count + ", total files: " + classReport.FilesCount + ")";

        foreach (var functionx in classx.ProyectFunctions)
        {
          ReportItem functionxItem = new ReportItem();

          functionxItem.Data = functionx.FunctionName + " (total: " + functionx.Count + ") => " + functionx.Parameters;

          foreach (var item in reportInitial.Items)
          {
            foreach (var item2 in item.Items)
            {
              foreach (var item3 in item2.Items)
              {
                if (item2.Data == classx.Name && item3.Data == functionx.FunctionName)
                {
                  var functionData = item.Data + " (" + item.Path + ":" + item3.LineNumber + ") => " + item3.LineCode;

                  var exist = functionxItem.Items.Where(x => x.Data == functionData).FirstOrDefault();

                  if (exist == null)
                  {
                    functionxItem.Items.Add(new ReportItem
                    {
                      Data = functionData
                    });
                  }
                }
              }
            }
          }

          classxItem.Items.Add(functionxItem);
        }

        response.Items.Add(classxItem);
      }

      return response;
    }

    public Report BuildReport(Proyect proyect)
    {
      Report response = new Report();

      List<string> exclude = proyect.ProyectFiles.Select(x => x.Path).ToList();

      foreach (var file in proyect.ProyectFiles)
      {
        // Encuentro los archivos que hagan referencia al namespace
        var filesNameSpace = searchService.Find(proyect.LocalDirectory + proyect.RelativePath, null, file.Namespace, exclude);

        if (filesNameSpace.Count == 0)
          return response;

        foreach (var item in filesNameSpace)
        {
          var code = File.ReadAllText(item.File.FullName, Encoding.UTF8);

          string path = item.File.FullName.Replace(proyect.LocalDirectory, "").Replace(@"\\", @"\");

          response.Items.Add(new ReportItem
          {
            Data = item.File.Name,
            Code = code.Replace(System.Environment.NewLine, "<br/>").Replace(" ", "&nbsp").Replace("\t", "  "),
            Path = path
          });


        }

        foreach (var classx in file.ProyectClasses)
        {

          if (classx.Name == "Bucket_Properties")
          { }

          // Encuentro los archivos que hagan referencia al namspace y la clase
          var fileClasses = searchService.Find(null, filesNameSpace, classx.Name + ".", exclude);
          var fileClasses2 = searchService.Find(null, filesNameSpace, classx.Name + "(", exclude);
          var fileClasses3 = searchService.Find(null, filesNameSpace, classx.Name + " ", exclude);
          var fileClasses4 = searchService.Find(null, filesNameSpace, "As " + classx.Name, exclude);

          foreach (var fc2 in fileClasses2)
          {
            if (!fileClasses.Select(x => x.File.Name).ToList().Contains(fc2.File.Name))
            {
              fileClasses.Add(fc2);
            }
          }

          foreach (var fc2 in fileClasses3)
          {
            if (!fileClasses.Select(x => x.File.Name).ToList().Contains(fc2.File.Name))
            {
              fileClasses.Add(fc2);
            }
          }

          foreach (var fc2 in fileClasses4)
          {
            if (!fileClasses.Select(x => x.File.Name).ToList().Contains(fc2.File.Name))
            {
              fileClasses.Add(fc2);
            }
          }

          var filesIn = fileClasses.Select(x => x.File.FullName.Replace(proyect.LocalDirectory, "")).Distinct().ToList();

          foreach (var item in fileClasses)
          {
            var algo = response.Items.Where(x => x.Data == item.File.Name).FirstOrDefault();

            algo.Code = algo.Code.Replace(classx.Name,
                     "<span style=\"background-color: yellow\">" + classx.Name + "</span>");

            string path = item.File.FullName.Replace(proyect.LocalDirectory, "").Replace(@"\\", @"\");

            response.Paths.Add(path.Substring(0, path.LastIndexOf(@"\")));



            algo.Items.Add(
               new ReportItem
               {
                 Data = classx.Name,
                 FilesCount = filesIn.Count,
                 FilesFindText = string.Join(",", filesIn),
                 FilesFind = filesIn
               });
          }

          foreach (var proyectFunction in classx.ProyectFunctions)
          {
            // Encuentro archivos que contengan todo lo anterior y la llamada a una funcion
            var filesResult = searchService.Find(null, fileClasses, proyectFunction.FunctionName + "(", exclude);

            foreach (var item in filesResult)
            {
              var itemsNamspace = response.Items.Where(x => x.Items.Count > 0).ToList();

              itemsNamspace = itemsNamspace.Where(x => x.Data == item.File.Name).ToList();

              foreach (var inspace in itemsNamspace)
              {
                foreach (var asd in inspace.Items)
                {
                  if (asd.Data == classx.Name)
                  {
                    var classProyect = proyect.ProyectFiles[0].ProyectClasses.Where(x => x.Name == classx.Name).FirstOrDefault();

                    classProyect.Count += 1;

                    var funcAlgo = classProyect.ProyectFunctions.Where(x => x.FunctionName == proyectFunction.FunctionName).FirstOrDefault();

                    funcAlgo.Count += 1;

                    inspace.Code = inspace.Code.Replace(proyectFunction.FunctionName,
                      "<span style=\"background-color: yellow\">" + proyectFunction.FunctionName + "</span>");

                    asd.Items.Add(new ReportItem
                    {
                      Data = proyectFunction.FunctionName,
                      LineCode = item.LineCode,
                      LineNumber = item.LineNumber
                    });
                  }
                }
              }
              // var updatear = itemsNamspace.it


              //foreach (var itemClass in itemsNamspace)
              //{
              //  if (itemClass.Data == classx.Name)
              //  {
              //    itemClass.Items.Add(new ReportItem
              //    {
              //      Data = proyectFunction.FunctionName
              //    });
              //  }
              //}

              //itemClass.Items.Where(x => x.Data == item.Name).FirstOrDefault().Items.Add(
              //new ReportItem
              //{
              //  Data = classx.Name
              //}
              //);



            }


          }
        }
      }

      var files = response.Items.Where(x => x.Items.Count > 0).ToList();

      //List<ReportItem> items = new List<ReportItem>();

      //foreach (var file in files)
      //{
      //  foreach (var classx in file.Items)
      //  {
      //    if (classx.Items.Count > 0)
      //    {
      //      items.Add(file);
      //    }
      //  }
      //}

      response.Items = files;

      response.UniquePaths = response.Paths.Distinct().ToList();

      foreach (var pathUnique in response.UniquePaths)
      {
        response.UniquePathsCount.Add(pathUnique, response.Paths.Where(x => x == pathUnique).Count());
      }

      return response;
    }

    public void WriteReportHtml(Report report, string reportName, string path, string destinationPath)
    {
      // var code = File.ReadAllText(path + reportName + ".html", Encoding.UTF8);

      //code = code.Replace("var dataAll = null;", "var dataAll = JSON.parse(" + JsonSerializer.Serialize(report)+");");

      // File.WriteAllText(destinationPath + "html\\" + reportName + "export.json", JsonSerializer.Serialize(report), Encoding.UTF8);

      // File.WriteAllText(destinationPath + "html\\" + reportName + "export.html", code, Encoding.UTF8);
    }

    public Proyect GenerateProyect(string path)
    {
      var response = new Proyect();

      response.ProyectsList.AddRange(new List<string>{
      "\\Wigos System\\WGC\\Kernel\\WSI.Common",
      "\\Wigos System\\WGC\\Kernel\\WSI.CommonA",
      "\\Wigos System\\WGC\\GUI"
      });

      response.ProyectFiles.Add(GetFile(path));

      return response;
    }

    private ProyectFile GetFile(string path)
    {
      var code = File.ReadLines(path, Encoding.UTF8).ToList();

      var response = new ProyectFile();

      response.Path = path;

      string className = string.Empty;

      List<ProyectFunction> functions = new List<ProyectFunction>();

      int lineNumber = 0;

      int totalLines = code.Count;

      foreach (var line in code)
      {
        if (line.IndexOf("namespace") >= 0)
        {
          response.Namespace = line.Substring(line.IndexOf("namespace") + 10).Trim();
        }
        else if ((line.IndexOf("class ") >= 0
          && ((line.IndexOf("//") > line.IndexOf("class ") || line.IndexOf("//") < 0))
          && line.IndexOf("\"") == -1) || lineNumber == (totalLines -1))
        {

          if (className != string.Empty)
          {
            response.ProyectClasses.Add(new ProyectClass
            {
              Name = className,
              ProyectFunctions = functions.ToList()
            });
          }

          try
          {

            var name = line.Substring(line.IndexOf("class ") + 6).Trim();

            if (name.IndexOf(" ") > 0)
            {
              name = name.Substring(0, name.IndexOf(" ")).Trim();
            }

            if (name.IndexOf(":") > 0)
            {
              name = name.Substring(0, name.IndexOf(":")).Trim();
            }

            className = name;

            functions = new List<ProyectFunction>();
          }
          catch { }


        }
        else if (line.ToLower().Trim().IndexOf("public") == 0 && line.ToLower().Trim().IndexOf("class") == -1)
        {
          var match = Regex.Match(line, "([a-z].) ([a-z]*)([^\\s-])([(].)", RegexOptions.IgnoreCase);

          var functionName = string.Empty;
          var parameters = string.Empty;

          if (match.Success)
          {
            if (line.IndexOf("public Dictionary<") > -1)
            {
              functionName = line.Substring(line.IndexOf(">") + 1).Trim();
              functionName = functionName.Substring(0, functionName.IndexOf(" ")).Trim();
            }
            else
            {
              functionName = match.Value.Substring(match.Value.IndexOf(" ")).Trim();
              functionName = functionName.Substring(0, functionName.IndexOf("(")).Trim();

              parameters = line.Substring(line.IndexOf("(")).Trim();

              if (line.IndexOf(")") == -1)
              {
                bool found = false;

                while (!found)
                {
                  for (int i = (lineNumber + 1); i < (code.Count() - 1); i++)
                  {

                    parameters += code[i].Trim().Replace(System.Environment.NewLine, " ");

                    if (code[i].IndexOf(")") > -1)
                    {
                      found = true;
                      break;
                    }
                  }
                }

              }
            }

            functions.Add(new ProyectFunction
            {
              FunctionName = functionName,
              Parameters = parameters
            });
          }
        }

        lineNumber += 1;
      }

      return response;
    }


  }
}
