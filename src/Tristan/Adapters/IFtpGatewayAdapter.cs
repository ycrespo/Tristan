using System.Collections.Generic;
using System.Threading.Tasks;
using Tristan.Core.Models;
using Tristan.Data.Models;

namespace Tristan.Adapters
{
    public interface IFtpGatewayAdapter
    {
        IEnumerable<Doc> GetProcessableDocs(string sourceDirectory, IEnumerable<Doc> accumulatedDocs, int chunk);
        Task<IEnumerable<Doc>> CopyDocsAsync(IEnumerable<Doc> docs);
        void Delete(IEnumerable<Doc> docs);
    }
}