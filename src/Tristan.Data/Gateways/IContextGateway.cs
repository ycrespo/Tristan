using System.Collections.Generic;
using System.Threading.Tasks;
using Tristan.Core.Models;

namespace Tristan.Data.Gateways
{
    public interface IContextGateway
    {
        Task<IEnumerable<Doc>> GetPendingDocsAsync();
        Task<IEnumerable<Doc>> SaveAsync(IEnumerable<Doc> entities);
        Task<IEnumerable<Doc>> UpdateAsync(IEnumerable<Doc> Docs);
        Task<IEnumerable<Doc>> DeleteAsync(IEnumerable<Doc> Docs);
    }
}