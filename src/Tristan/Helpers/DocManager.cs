using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tristan.Core.Gateways;
using Tristan.Core.Models;

namespace Tristan.Helpers
{
    public class DocManager : IDocManager
    {
        private readonly ILogger<DocManager> _logger;
        private readonly IFtpGateway _ftpGateway;

        public DocManager(IFtpGateway ftpGateway, ILogger<DocManager> logger)
        {
            _logger = logger;
            _ftpGateway = ftpGateway;
        }

        public IEnumerable<Doc> GetProcessableDocs(string sourceDirectory, IEnumerable<Doc> accumulatedDocs, int chunk)
        {
            var paths = _ftpGateway.RetrievePendingDocs(sourceDirectory);
            if (paths.HasError()) ;  // Silent error, nothing to do! Photos will reprocess next schedule.

            var allDocs = GetDocs(paths.Success).ToList();
            var oldDocs = accumulatedDocs.ToList();

            return allDocs.Except(oldDocs).Take(chunk - oldDocs.Count);
        }

        public async Task<IEnumerable<Doc>> CopyDocsAsync(IEnumerable<Doc> docs)
        {
            var copiedDocs = new List<Doc>();

            foreach (var doc in docs)
            {
                var destination = Path.Combine(doc.DestinationDir, $"{doc.Filename}{doc.Extension}");
                var result = await _ftpGateway.CopyDocAsync(doc.Path, destination);
                if (result.HasError())
                    continue; // Silent error, nothing to do! No copied photos will reprocess next schedule.

                doc.Moved = true;
                copiedDocs.Add(doc);
            }

            return copiedDocs;
        }

        public void Delete(IEnumerable<Doc> docs)
        {
            var sources = docs.Select(d => d.Path);
            var result = _ftpGateway.DeleteDocs(sources);

            if (result.HasError()) ; // Silent error, nothing to do! No deleted photos will reprocess next schedule.
        }
        
        
        private IEnumerable<Doc> GetDocs(IEnumerable<string> paths) =>
            from path in paths 
            let filenameResult = Result.Try(() => 
                Path.GetFileNameWithoutExtension(path)?.Trim(), ex => 
               _logger.LogError(ex, $"Cannot retrieve filename for path: {path}.")) 
            let extensionResult = Result.Try(() => 
                Path.GetExtension(path)?.Trim(), ex => 
                _logger.LogError(ex, $"Cannot retrieve extension for path: {path}.")) 
            where !extensionResult.HasError() && !filenameResult.HasError() 
            select new Doc { Path = path, Filename = filenameResult.Success, Extension = extensionResult.Success };
    }
}