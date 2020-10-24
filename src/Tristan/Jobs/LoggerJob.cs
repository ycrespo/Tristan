using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using Tristan.Data.Gateways;
using Tristan.Data.Models;

namespace Tristan.Jobs
{
    [DisallowConcurrentExecution]
    public class LoggerJob : IJob
    {
        private readonly ILogger<LoggerJob> _logger;
        private readonly IServiceProvider _provider;

        public LoggerJob(ILogger<LoggerJob> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            // Create a new scope
            using var scope = _provider.CreateScope();
            var contextGateway = scope.ServiceProvider.GetService<IContextGateway>();
            // Resolve the Scoped service
            if (contextGateway != null)
            {
                var tblDoc = new List<TblDoc>
                {
                    new TblDoc
                    {
                        Id = Guid.Empty,
                        Filename = "fileName",
                        Extension = ".pdf",
                        Path = "./",
                        NumberOfRetry = 0,
                        Moved = false,
                        DestinationDir = string.Empty,
                        OccurredOn = DateTime.Now
                    }
                };
            
                _logger.LogInformation("Writing Data", DateTimeOffset.Now);
                await contextGateway.SaveAsync(tblDoc);
                
                _logger.LogInformation("Reading Data", DateTimeOffset.Now);
                var newTblDoc = contextGateway.ReadAsync();
                _logger.LogInformation($"Reading Data: the filename is {newTblDoc.FirstOrDefault()?.Filename}", DateTimeOffset.Now);
                
                _logger.LogInformation("Updating Data", DateTimeOffset.Now);
                tblDoc.FirstOrDefault().Filename = "NewFilename";
                await contextGateway.UpdateAsync(tblDoc);
                
                _logger.LogInformation("Reading Data", DateTimeOffset.Now);
                newTblDoc = contextGateway.ReadAsync();
                _logger.LogInformation($"Reading Data: the filename is {newTblDoc.FirstOrDefault()?.Filename}", DateTimeOffset.Now);

                _logger.LogInformation("Deleting Data", DateTimeOffset.Now);
                await contextGateway.DeleteAsync(tblDoc);
            }
            
        }
    }
}