using System.Collections.Generic;
using System.IO;

namespace Tristan.Core.Services
{
    public class SystemDirectoryService : IDirectoryService
    {
        public bool Exists(string path) => Directory.Exists(path);

        public void CreateDir(string path) => Directory.CreateDirectory(path);

        public void DeleteDir(string path) => Directory.Delete(path);

        public IEnumerable<string> GetFiles(string path) => Directory.GetFiles(path);

        public string GetFinalDestination(string rooDir, string fileName)
        {
            if (!Exists(rooDir))
                return string.Empty;
            
            var index = fileName.IndexOf(value: '-');
            var dirName =  fileName.Substring(startIndex: 0, index - 1);
            var destinationPath =  Path.Combine(rooDir, dirName);
            
            if (!Exists(destinationPath)) 
                CreateDir(destinationPath);

            return destinationPath;
        }
    }
}