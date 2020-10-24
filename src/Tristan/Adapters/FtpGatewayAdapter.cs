using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tristan.Core.Gateways;
using Tristan.Core.Models;
using Tristan.Factories;

namespace Tristan.Adapters
{
    public class FtpGatewayAdapter : IFtpGatewayAdapter
    {
        private readonly IFtpGateway _ftpGateway;
        private readonly IDocsFactory _docsFactory;

        public FtpGatewayAdapter(IFtpGateway ftpGateway, IDocsFactory docsFactory)
        {
            _ftpGateway = ftpGateway;
            _docsFactory = docsFactory;
        }

        public IEnumerable<Doc> GetProcessableDocs(string sourceDirectory, IEnumerable<Doc> accumulatedDocs, int chunk)
        {
            var paths = _ftpGateway.RetrievePendingDocs(sourceDirectory);
            if (paths.HasError())
            {
                ; // Silent error, nothing to do! Photos will reprocess next schedule.
            }

            var allDocs = _docsFactory.GetDocs(paths.Success).ToList();
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
                {
                    continue; // Silent error, nothing to do! No copied photos will reprocess next schedule.
                }

                doc.Moved = true;
                copiedDocs.Add(doc);
            }

            return copiedDocs;
        }

        public void Delete(IEnumerable<Doc> docs)
        {
            var sources = docs.Select(d => d.Path);
            var result = _ftpGateway.DeleteDocs(sources);

            if (result.HasError())
            {
                ; // Silent error, nothing to do! No deleted photos will reprocess next schedule.
            }
        }
    }
}