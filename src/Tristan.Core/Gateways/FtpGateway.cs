using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tristan.Core.Models;
using Tristan.Core.Services;

namespace Tristan.Core.Gateways
{
    public class FtpGateway : IFtpGateway
    {
        private readonly ILogger<FtpGateway> _logger;
        private readonly IDirectoryService _directoryService;

        public FtpGateway(ILogger<FtpGateway> logger, IDirectoryService directoryService)
        {
            _logger = logger;
            _directoryService = directoryService;
        }

        public IResult<IEnumerable<string>, IEnumerable<Error>> RetrievePendingDocs(string source)
        {
            var pathsResult = Result.Try(
                () => _directoryService.GetFiles(source),
                ex => _logger.LogError(ex, $"Cannot get Docs from: {source}.")
            );
            
            return Result<IEnumerable<Error>>.Success(pathsResult.Success);
        }

        public async Task<IResult<Unit, Error>> CopyDocAsync(string source, string destination)
        {
            var destinationDir = Path.GetDirectoryName(destination);
            if(!_directoryService.Exists(destinationDir))
                _directoryService.CreateDir(destinationDir);
                
            return await Result.Try(async () =>
                {
                    await using var sourceStream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
                    await using var destinationStream = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);

                    await sourceStream.CopyToAsync(destinationStream);
                },
                ex => _logger.LogError(ex, $"Copy Failed! from {source} path to {destination} path"));
        }

        public async Task<IResult<IEnumerable<string>, IEnumerable<Error>>> CopyDocsAsync(IEnumerable<string> sources, string destination)
        {
            var success = new List<string>();
            var errors = new List<Error>();

            foreach (var source in sources)
            {
                var result = await CopyDocAsync(source, destination);
                if (result.HasError())
                {
                    errors.Add(new Error.CopyFailed(source));
                }
                else
                {
                    success.Add(source);
                }
            }
            return new Result<IEnumerable<string>, IEnumerable<Error>>(success, errors);
        }

        public IResult<IEnumerable<string>, IEnumerable<Error>> DeleteDocs(IEnumerable<string> sources)
        {
            var success = new List<string>();
            var errors = new List<Error>();

            foreach (var source in sources)
            {
                var result = Result.Try(
                    () => File.Delete(source),
                    ex => _logger.LogError(ex, $"Delete Failed! {source}"));

                if (result.HasError())
                {
                    errors.Add(new Error.DeleteFailed(source));
                }
                else
                {
                    success.Add(source);
                }
            }

            return new Result<IEnumerable<string>, IEnumerable<Error>>(success, errors);
        }
    }
}