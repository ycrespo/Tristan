using System.Collections.Generic;
using System.Threading.Tasks;
using Tristan.Core.Models;
using Tristan.Data.Models;

namespace Tristan.Data.Gateways
{
    public interface IContextGateway
    {
        Task<IResult<IEnumerable<TblDoc>, Error.Exceptional>> GetPendingDocs();
        Task<IEnumerable<TblDoc>> SaveAsync(IEnumerable<TblDoc> entities);
        Task<IEnumerable<TblDoc>> UpdateAsync(IEnumerable<TblDoc> tblDocs);
        Task<IEnumerable<TblDoc>> DeleteAsync(IEnumerable<TblDoc> tblDocs);
    }
}