using System.Collections.Generic;

namespace Tristan.Core.Services
{
    public interface IDirectoryService
    {
        bool Exists(string path);

        void CreateDir(string path);

        void DeleteDir(string path);

        IEnumerable<string> GetFiles(string path);

        public string GetFinalDestination(string rooDir, string fileName);
    }
}