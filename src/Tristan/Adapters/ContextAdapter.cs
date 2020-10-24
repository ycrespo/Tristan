using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tristan.Core.Gateways;
using Tristan.Core.Models;
using Tristan.Data.Gateways;
using Tristan.Data.Models;

namespace Tristan.Adapters
{
    public class ContextAdapter : IContextAdapter
    {
        private readonly IContextGateway _contextGateway;

        public ContextAdapter(IContextGateway contextGateway)
        {
            _contextGateway = contextGateway;
        }

        public async Task<IEnumerable<TblDoc>> GetAccumulatedDocsAsync()
        {
            var result = await _contextGateway.GetPendingDocs();
            
            return result.Success ??  new List<TblDoc>();
        }

        public async Task<IEnumerable<TblDoc>> SavePendingDocsAsync(IEnumerable<Doc> docs)
        {
            var tblDocs = docs.Select(Mapper.Map);

            var result = await _contextGateway.SaveAsync(tblDocs);

            return result ?? new List<TblDoc>();
        }

        public async Task<IEnumerable<Doc>> UpdateDocsAsync(IEnumerable<TblDoc> tblDocs,IEnumerable<Doc> docs)
        {
            tblDocs = tblDocs.Select(tb =>
            {
                var doc = docs.FirstOrDefault(d => d.Id == tb.Id);
                return doc == default
                    ? tb
                    : Mapper.Map(tb, doc);
            });
            
            var result = await _contextGateway.UpdateAsync(tblDocs);
            
            return result is null
                ? new List<Doc>()
                : result.Select(Mapper.Map);
        }

        public async Task<IEnumerable<Doc>> DeleteDocsAsync(IEnumerable<Doc> docs)
        {
            var tblDocs = docs.Select(Mapper.Map);

            var result = await _contextGateway.DeleteAsync(tblDocs);

            return result is null
                ? new List<Doc>()
                : result.Select(Mapper.Map);
        }
    }
}