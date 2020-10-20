using System;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;

namespace Tristan.Jobs
{
    [DisallowConcurrentExecution]
    public class LoggerJob : IJob
    {
        private readonly ILogger<LoggerJob> _logger;
        
        public LoggerJob(ILogger<LoggerJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            return Task.CompletedTask;
        }
    }

}