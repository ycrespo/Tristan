using System.Collections.Generic;
using System.Threading.Tasks;
using Tristan.Core.Models;

namespace Tristan.Helpers
{
    public interface IDocManager
    {
        IEnumerable<Doc> GetProcessableDocs(string sourceDirectory, IEnumerable<Doc> accumulatedDocs, int chunk);
        Task<IEnumerable<Doc>> CopyDocsAsync(IEnumerable<Doc> docs);
        void Delete(IEnumerable<Doc> docs);
    }
}