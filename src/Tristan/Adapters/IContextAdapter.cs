using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Tristan.Core.Models;
using Tristan.Data.Models;

namespace Tristan.Adapters
{
    public interface IContextAdapter
    {
        Task<IEnumerable<TblDoc>> GetAccumulatedDocsAsync();
        Task<IEnumerable<TblDoc>> SavePendingDocsAsync(IEnumerable<Doc> docs);
        Task<IEnumerable<Doc>> UpdateDocsAsync(IEnumerable<TblDoc> tblDocs, IEnumerable<Doc> docs);
        Task<IEnumerable<Doc>> DeleteDocsAsync(IEnumerable<Doc> sent);
    }
}