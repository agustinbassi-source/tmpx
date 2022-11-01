using Service.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Service
{
  public class SearchService
  {

    public List<FindResponse> Find(string path, List<FindResponse> files, string text, List<string> excluede)
    {

      // Modify this path as necessary.  
      string startFolder = path;

      // Take a snapshot of the file system.  

      List<System.IO.FileInfo> fileList;
      // This method assumes that the application has discovery permissions  
      // for all folders under the specified path.  
      if (files != null)
      {
        fileList = files.Select(x=> x.File).ToList();
      }
      else
      {
        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(startFolder);

        fileList = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories)
            .Where(s => s.FullName.EndsWith(".cs") || s.FullName.EndsWith(".vb")).ToList();
      }

      List<FindResponse> fileResponse = new List<FindResponse>();

      string searchTerm = text;

      // Search the contents of each file.  
      // A regular expression created with the RegEx class  
      // could be used instead of the Contains method.  
      // queryMatchingFiles is an IEnumerable<string>.  
      var queryMatchingFiles =
          from file in fileList
            // where file.Extension == ".vb"  
          let fileText = GetFileText(file.FullName)
          where fileText.Contains(searchTerm)
          select file.FullName;

      // Execute the query.  
      // Console.WriteLine("The term \"{0}\" was found in:", searchTerm);
      foreach (string filename in queryMatchingFiles)
      {
        if (!excluede.Contains(filename))
        {
          var textAll = GetFileText(filename);

          var dataTuple = GetLineNumber(textAll, searchTerm);
 
          var fileX = new System.IO.FileInfo(filename);

          fileResponse.Add(new FindResponse
          {
            File = fileX,
            LineCode = dataTuple.Item2,
            LineNumber = dataTuple.Item1

          });  
        }
        // Console.WriteLine(filename);
      }

      // Keep the console window open in debug mode.  
      // Console.WriteLine("Press any key to exit");
      // Console.ReadKey();

      // return queryMatchingFiles.ToList();

      return fileResponse;
    }

    public static (int, string) GetLineNumber(string text, string lineToFind, StringComparison comparison = StringComparison.CurrentCulture)
    {
      int lineNum = 0;
      using (StringReader reader = new StringReader(text))
      {
        string line;
        while ((line = reader.ReadLine()) != null)
        {
          lineNum++;
          if (line.Contains(lineToFind, comparison))
            return  (lineNum, line);
        }
      }
      return (-1,"");
    }

    // Read the contents of the file.  
    static string GetFileText(string name)
    {
      string fileContents = String.Empty;

      // If the file has been deleted since we took
      // the snapshot, ignore it and return the empty string.  
      if (System.IO.File.Exists(name))
      {
        fileContents = System.IO.File.ReadAllText(name);
      }
      return fileContents;
    }
  }
}
