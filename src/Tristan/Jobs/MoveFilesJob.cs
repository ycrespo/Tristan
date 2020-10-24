using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Tristan.Adapters;
using Tristan.Core.ExtensionMethods;
using Tristan.Core.Validators;
using Tristan.Data.Gateways;
using Tristan.Settings;

namespace Tristan.Jobs
{
    public class MoveFilesJob : IJob
    {
        private readonly ILogger<MoveFilesJob> _logger;
        private readonly IFtpGatewayAdapter _ftpGatewayAdapter;
        private readonly IServiceProvider _service;
        private readonly IDocValidator _validator;
        private readonly TristanSettings _options;

        public MoveFilesJob(ILogger<MoveFilesJob> logger, IFtpGatewayAdapter ftpGatewayAdapter, IServiceProvider service, IDocValidator validator, IOptions<TristanSettings> options)
        {
            _logger = logger;
            _ftpGatewayAdapter = ftpGatewayAdapter;
            _service = service;
            _validator = validator;
            _options = options.Value;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = _service.CreateScope();
            var contextDb = scope.ServiceProvider.GetService<IContextAdapter>();
           
            //Get docs that can be reprocessed because of an error
            if (contextDb != null)
            {
                var accumulatedDocs = (await contextDb.GetAccumulatedDocsAsync()).ToList();
                //Get all files in folder
                var newDocs = _ftpGatewayAdapter.GetProcessableDocs(_options.SourceDirectory, accumulatedDocs.Select(Mapper.Map), _options.DocChunk).ToList();
                //Save new docs a DB
                var tblDocs = (await contextDb.SavePendingDocsAsync(newDocs)).ToList();
                //Complete the list of processable jobs
                tblDocs.AddRange(accumulatedDocs);
            
                if (!tblDocs.Any())
                    return;

                //Increment the number of retry
                var docs = tblDocs
                    .Select(Mapper.Map)
                    .IncrementNumberOfRetry()
                    .ToList();
            
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

                _logger.LogInformation("Copying valid files.");
                var copiedValidDocs = await _ftpGatewayAdapter.CopyDocsAsync(validDocs);

                _logger.LogInformation("Updating database for valid files");
                await contextDb.UpdateDocsAsync(tblDocs, copiedValidDocs);

                _logger.LogInformation("Copying invalid files.");
                var copiedInValidDocs = await _ftpGatewayAdapter.CopyDocsAsync(invalidDocs);

                _logger.LogInformation("Updating database for invalid files");
                await contextDb.UpdateDocsAsync(tblDocs, copiedInValidDocs);

                _logger.LogInformation("Deleting files.");
                _ftpGatewayAdapter.Delete(docs);
            }
        }       
    }           
}               