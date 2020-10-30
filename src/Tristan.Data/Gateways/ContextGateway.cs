using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tristan.Core.ExtensionMethods;
using Tristan.Data.DataAccess;
using Tristan.Core.Models;

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
        
        public async Task<IEnumerable<Doc>> GetPendingDocsAsync()
        {
            var result = await Result.Try(
                async () => (await _context.Doc.Where(doc => !doc.Moved).ToListAsync()).AsEnumerable(),
                ex => _logger.LogError(ex, "Cannot get pending photos from database."));
            
            return result.Success ?? new List<Doc>();
        }
        
        
        public async Task<IEnumerable<Doc>> SaveAsync(IEnumerable<Doc> entities)
        {
            var Docs = entities.SetOccurredOn().ToList();
            
            foreach (var Doc in Docs)
            {
                var inserted = await _context.AddAsync(Doc);

                Doc.Id = inserted.Entity.Id;
            }
            
            var result = await SaveChangesAsync("Cannot persist data to the database!!!");
            
            return result.HasError()
                ? new List<Doc>()
                : Docs;
        }
        
        public async Task<IEnumerable<Doc>> UpdateAsync(IEnumerable<Doc> Docs)
        {
            var entities = Docs.SetOccurredOn().ToList();

            foreach (var doc in entities)
            {
                _context.Doc.Update(doc);
            }
            
            var result = await SaveChangesAsync("Can not Update documents in database!!!"); 

            return result.HasError()
                ? new List<Doc>()
                : entities;
        }
        
        public async Task<IEnumerable<Doc>> DeleteAsync(IEnumerable<Doc> Docs)
        {
            var entities = Docs.ToList();

            _context.Doc.RemoveRange(entities);

            var result = await SaveChangesAsync("Can not delete documents in database!!!");

            return result.HasError()
                ? new List<Doc>()
                : entities;
        }
        
        private Task<IResult<int,Error.Exceptional>> SaveChangesAsync(string errorMessage)
        => Result.Try(
            async () => await _context.SaveChangesAsync(),
            ex => _logger.LogError(ex, errorMessage));

    }
}