using System.Collections.Generic;
using System.Threading.Tasks;
using Tristan.Core.Models;

namespace Tristan.Core.Gateways
{
    public interface IFtpGateway
    {
        IResult<IEnumerable<string>, IEnumerable<Error>> RetrievePendingDocs(string source);

        Task<IResult<Unit, Error>> CopyDocAsync(string source, string destination);
        Task<IResult<IEnumerable<string>, IEnumerable<Error>>> CopyDocsAsync(IEnumerable<string> sources, string destination);

        IResult<IEnumerable<string>, IEnumerable<Error>> DeleteDocs(IEnumerable<string> sources);
    }
}