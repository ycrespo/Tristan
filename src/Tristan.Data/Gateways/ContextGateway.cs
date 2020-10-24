using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tristan.Data.DataAccess;
using Tristan.Data.Models;
using Tristan.Data.ExtensionMethods;

namespace Tristan.Data.Gateways
{
    public class ContextGateway : IContextGateway
    {
        private readonly TristanContext _context;
        private readonly ILogger<TristanContext> _logger;

        public ContextGateway(TristanContext context, ILogger<TristanContext> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<TblDoc> ReadAsync() =>  _context.TblDoc.Local;

        public async Task SaveAsync(IEnumerable<TblDoc> entities)
        {
            var tblDocs = entities.SetOccurredOn().ToList();
            
            foreach (var tblDoc in tblDocs)
            {
                var inserted = await _context.AddAsync(tblDoc);

                tblDoc.Id = inserted.Entity.Id;
            }

            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateAsync(IEnumerable<TblDoc> tblDocs)
        {
            var entities = tblDocs.SetOccurredOn().ToList();

            foreach (var doc in entities)
            {
                _context.TblDoc.Update(doc);
            }
            
            await _context.SaveChangesAsync();
        }
        
        public async Task DeleteAsync(IEnumerable<TblDoc> tblDocs)
        {
            var entities = tblDocs.ToList();

            _context.TblDoc.RemoveRange(entities);

            await _context.SaveChangesAsync();
        }
    }
}