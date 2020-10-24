using System.Collections.Generic;
using Tristan.Core.Models;

namespace Tristan.Factories
{
    public interface IDocsFactory
    {
        IEnumerable<Doc> GetDocs(IEnumerable<string> paths);
    }
}