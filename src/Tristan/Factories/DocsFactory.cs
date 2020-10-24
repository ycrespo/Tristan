using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Tristan.Core.Models;

namespace Tristan.Factories
{
    public class DocsFactory : IDocsFactory
    {
        private readonly ILogger<DocsFactory> _logger;

        public DocsFactory(ILogger<DocsFactory> logger)
        {
            _logger = logger;
        }
        
        public IEnumerable<Doc> GetDocs(IEnumerable<string> paths) =>
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