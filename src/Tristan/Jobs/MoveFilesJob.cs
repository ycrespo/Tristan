using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Tristan.Core.ExtensionMethods;
using Tristan.Core.Models;
using Tristan.Core.Validators;
using Tristan.Data.Gateways;
using Tristan.Helpers;
using Tristan.Settings;

namespace Tristan.Jobs
{
    [DisallowConcurrentExecution]
    public class MoveFilesJob : IJob
    {
        private readonly ILogger<MoveFilesJob> _logger;
        private readonly IDocManager _docManager;
        private readonly IServiceProvider _service;
        private readonly IDocValidator _validator;
        private readonly TristanSettings _options;
        private IContextGateway _contextDb;

        public MoveFilesJob(ILogger<MoveFilesJob> logger, IDocManager docManager, IServiceProvider service, IDocValidator validator, IOptions<TristanSettings> options)
        {
            _logger = logger;
            _docManager = docManager;
            _service = service;
            _validator = validator;
            _options = options.Value;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = _service.CreateScope();
            _contextDb = scope.ServiceProvider.GetService<IContextGateway>();
           
            //Get docs that can be reprocessed because of an error
            if (_contextDb != null)
            {
                var accumulatedDocs = (await _contextDb.GetPendingDocsAsync()).ToList();
                //Get all files in folder
                var newDocs = _docManager.GetProcessableDocs(_options.SourceDirectory, accumulatedDocs, _options.DocChunk).ToList();
                //Save new docs a DB
                var docs = (await _contextDb.SaveAsync(newDocs)).ToList();
                //Complete the list of processable jobs
                docs.AddRange(accumulatedDocs);
            
                if (!docs.Any())
                    return;
                
                //Increment the number of retry
                docs.IncrementNumberOfRetry();
            
                //validate docs list
                var validDocs = docs
                    .Where(doc => _validator.Validate(doc))
                    .GetValidDestinations(_options.DestinationDirectory)
                    .ToList();

                //Get invalid docs
                var invalidDocs = docs
                    .Except(validDocs)
                    .GetInValidDestinations(_options.ErrorsDirectory)
                    .ToList();

                _logger.LogInformation("Processing valid files.");
                await ProcessDocsAsync(validDocs);
                
                _logger.LogInformation("Processing invalid files.");
                await ProcessDocsAsync(invalidDocs);
            }
        }
        
        private async Task ProcessDocsAsync(IEnumerable<Doc> docs)
        {
            _logger.LogInformation("Copying files.");
            var copiedDocs = (await _docManager.CopyDocsAsync(docs)).ToList();

            _logger.LogInformation("Updating database");
            await _contextDb.UpdateAsync(copiedDocs);
                
            _logger.LogInformation("Deleting files.");
            _docManager.Delete(copiedDocs);
        }
    }           
}               