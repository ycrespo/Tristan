using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tristan.Data.DataAccess;
using Tristan.Data.Models;
using Tristan.Core.Models;
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
        
        public Task<IResult<IEnumerable<TblDoc>, Error.Exceptional>> GetPendingDocs()
        {
            return Result.Try(
                async () => (await _context.TblDoc.Where(doc => !doc.Moved).ToListAsync()).AsEnumerable(),
                ex => _logger.LogError(ex, "Cannot get pending photos from database."));
        }
        
        
        public async Task<IEnumerable<TblDoc>> SaveAsync(IEnumerable<TblDoc> entities)
        {
            var tblDocs = entities.SetOccurredOn().ToList();
            
            foreach (var tblDoc in tblDocs)
            {
                var inserted = await _context.AddAsync(tblDoc);

                tblDoc.Id = inserted.Entity.Id;
            }
            
            var result = await SaveChangesAsync("Cannot persist data to the database!!!");
            
            return result.HasError()
                ? new List<TblDoc>()
                : tblDocs;
        }
        
        public async Task<IEnumerable<TblDoc>> UpdateAsync(IEnumerable<TblDoc> tblDocs)
        {
            var entities = tblDocs.SetOccurredOn().ToList();

            foreach (var doc in entities)
            {
                _context.TblDoc.Update(doc);
            }
            
            var result = await SaveChangesAsync("Can not Update documents in database!!!"); 

            return result.HasError()
                ? new List<TblDoc>()
                : entities;
        }
        
        public async Task<IEnumerable<TblDoc>> DeleteAsync(IEnumerable<TblDoc> tblDocs)
        {
            var entities = tblDocs.ToList();

            _context.TblDoc.RemoveRange(entities);

            var result = await SaveChangesAsync("Can not delete documents in database!!!");

            return result.HasError()
                ? new List<TblDoc>()
                : entities;
        }
        
        private Task<IResult<int,Error.Exceptional>> SaveChangesAsync(string errorMessage)
        => Result.Try(
            async () => await _context.SaveChangesAsync(),
            ex => _logger.LogError(ex, errorMessage));

    }
}