using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

namespace Service
{
  public class SearchService
  {

    public List<System.IO.FileInfo> Find(string path, List<System.IO.FileInfo> files, string text, List<string> excluede)
    {

      // Modify this path as necessary.  
      string startFolder = path;

      // Take a snapshot of the file system.  
    
      List<System.IO.FileInfo> fileList ;
      // This method assumes that the application has discovery permissions  
      // for all folders under the specified path.  
      if (files != null)
      {
        fileList = files;
      }
      else
      {
        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(startFolder);

        fileList = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories)
            .Where(s => s.FullName.EndsWith(".cs") || s.FullName.EndsWith(".vb")).ToList();
      }

      List<System.IO.FileInfo> fileResponse = new List<System.IO.FileInfo>();

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
      Console.WriteLine("The term \"{0}\" was found in:", searchTerm);
      foreach (string filename in queryMatchingFiles)
      {
        if (!excluede.Contains(filename))
        {
          fileResponse.Add(new System.IO.FileInfo(filename));
        }
        // Console.WriteLine(filename);
      }

      // Keep the console window open in debug mode.  
      // Console.WriteLine("Press any key to exit");
      // Console.ReadKey();

      // return queryMatchingFiles.ToList();

      return fileResponse;
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
