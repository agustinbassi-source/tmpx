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



    public Report BuildReport3(Project proyect)
    {
      var json = string.Empty;
      using (StreamReader r = new StreamReader(@"./Resources/reportexport.json"))
      {
        json = r.ReadToEnd();
      }

      var reportInitial = BuildReport(proyect);

      Report response = new Report();

      response.Paths = reportInitial.Paths.OrderBy(x => x).ToList();

      response.UniquePaths = reportInitial.UniquePaths.OrderBy(x => x).ToList();

      response.UniquePathsCount = reportInitial.UniquePathsCount;

      var file = proyect.ProyectFiles[0];

      var levelOther = new ReportItem
      {
        Data = "Other files",
        DataRaw = "Others"
      };

      foreach (var proyectx in proyect.ProyectsList)
      {
        var level1 = new ReportItem
        {
          Data = proyectx.Substring(proyectx.LastIndexOf(@"\") + 1),
          DataRaw = proyectx
        };

        int level1cCount = 0;
        int level1cCountFunctions = 0;

        foreach (ReportItem filex in reportInitial.Items)
        {
          if (filex.Path.Contains(proyectx + @"\"))
          {
            filex.Found = true;

            foreach (var classx in filex.Items)
            {
              foreach (var functionx in classx.Items)
              {

                if (!level1.Items.Select(x => x.DataRaw).Contains(functionx.Data))
                {
                  level1cCount++;

                  var level2 = new ReportItem
                  {
                    Data = classx.Data + "." + functionx.Data,
                    DataRaw = functionx.Data,
                  };

                  int level2cCount = 0;

                  var filex2items = reportInitial.Items.Where(x => x.Path.Contains(proyectx)).ToList();

                  foreach (var filex2 in filex2items)
                  {
                    foreach (var classx2 in filex2.Items)
                    {
                      if (classx2.Data == classx.Data)
                      {
                        foreach (var functionx2 in classx2.Items)
                        {
                          if (functionx2.Data == functionx.Data && !level2.Items.Select(x => x.DataRaw).Contains(filex2.Data))
                          {
                            level2cCount++;
                            level1cCountFunctions++;

                            level2.Items.Add(new ReportItem
                            {
                              Data = filex2.Path + ":" + functionx2.LineNumber + " => " + functionx2.LineCode,
                              DataRaw = filex2.Data
                            });
                          }
                        }
                      }
                    }
                  }

                  level2.Data += " (Files in use: " + level2cCount + ")";
                  level1.Items.Add(level2);
                }
              }


            }
          }



        }

        level1.Data += " (Functions in use: " + level1cCount + ", Files in use: " + level1cCountFunctions + ")";
        response.Items.Add(level1);
      }

      foreach (var item in reportInitial.Items.Where(x => !x.Found).ToList())
      {
        levelOther.Items.Add(new ReportItem
        {
          Data = item.Path,
          DataRaw = item.Data
        });
      }

      response.Items.Add(levelOther);

      response.Items = response.Items.Where(x => x.Items.Count > 0).ToList();

      return response;
    }


    public Report BuildReport2(Project proyect)
    {
      var json = string.Empty;
      using (StreamReader r = new StreamReader(@"./Resources/reportexport.json"))
      {
        json = r.ReadToEnd();
      }


      var reportInitial = BuildReport(proyect, json);

      Report response = new Report();

      response.Paths = reportInitial.Paths.OrderBy(x => x).ToList();

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

    public Report BuildReport(Project proyect)
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

    public Report BuildReport(Project proyect, string jsonString)
    {
      Report response = new Report();

      response = JsonSerializer.Deserialize<Report>(jsonString);


      return response;
    }

    public void WriteReportHtml(Report report, string reportName, string path, string destinationPath)
    {
      // var code = File.ReadAllText(path + reportName + ".html", Encoding.UTF8);

      //code = code.Replace("var dataAll = null;", "var dataAll = JSON.parse(" + JsonSerializer.Serialize(report)+");");

     File.WriteAllText(destinationPath + "html\\" + reportName + "export.json", JsonSerializer.Serialize(report), Encoding.UTF8);

      // File.WriteAllText(destinationPath + "html\\" + reportName + "export.html", code, Encoding.UTF8);
    }

    public Project GenerateProyect(string path)
    {
      var response = new Project();

      response.ProyectsList.AddRange(new List<string>{
      "\\Wigos System\\WGC\\Kernel\\WSI.Common",
      "\\Wigos System\\WGC\\Kernel\\WSI.CommonA",
      "\\Wigos System\\WGC\\GUI",
      "\\Wigos System\\WGC\\Kernel\\WSI.WCPA",
      "\\Wigos System\\WGC\\Kernel\\WSI.WCP",
      "\\Wigos System\\EveriCompliance.Test.Console",
      "\\Wigos System\\WSI.Cashier",
      "\\Wigos System\\WSI.PinPad.Service",
      "\\Wigos System\\WSI.Service",
      "\\Wigos System\\WSI.SQLBusinessLogic",
      "\\Wigos System\\WSI.WCP_Protocol",
      "\\Wigos System\\WSI.WWP_Client",
      "\\Wigos System\\WSI.WWP_Server",
      "\\Wigos System\\FB",
      "\\Wigos System\\PokerAtlas",
      "\\Wigos System\\WSI.WWP_Client",
      "\\Wigos System\\WSI.WWP_Server",
      "\\Wigos System\\WSPoints",
      "\\Wigos System\\Buckets",
      "\\Wigos System\\Wigos.Components\\Wigos.Components.Buckets",
      "\\Wigos System\\Wigos.Components\\Wigos.Components.Customers",
      "\\Wigos System\\Wigos.ReceptionApi",
      "\\Wigos System\\WSI.WWP_Server",
      "\\Wigos System\\WSPoints",
      "\\Wigos System\\C2GO",
      "\\Wigos System\\Wigos.Business\\FoodAndBeverage",
      "\\Wigos System\\Wigos.Components\\Wigos.Components.Buckets",
      "\\Wigos System\\Wigos.Components\\Wigos.Components.PACIN",
     });

      response.ProyectsList = response.ProyectsList.Distinct().OrderBy(x=> x).ToList();

      response.ProyectFiles.Add(GetFile(path));

      return response;
    }

    private ProjectFile GetFile(string path)
    {
      var code = File.ReadLines(path, Encoding.UTF8).ToList();

      var response = new ProjectFile();

      response.Path = path;

      string className = string.Empty;

      List<ProjectFunction> functions = new List<ProjectFunction>();

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
          && line.IndexOf("\"") == -1) || lineNumber == (totalLines - 1))
        {

          if (className != string.Empty)
          {
            response.ProyectClasses.Add(new ProjectClass
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

            functions = new List<ProjectFunction>();
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

            functions.Add(new ProjectFunction
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
