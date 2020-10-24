using System.Collections.Generic;
using System.Threading.Tasks;
using Tristan.Data.Models;

namespace Tristan.Data.Gateways
{
    public interface IContextGateway
    {
        IEnumerable<TblDoc> ReadAsync();
        Task SaveAsync(IEnumerable<TblDoc> entities);
        Task UpdateAsync(IEnumerable<TblDoc> tblDocs);
        Task DeleteAsync(IEnumerable<TblDoc> tblDocs);
    }
}