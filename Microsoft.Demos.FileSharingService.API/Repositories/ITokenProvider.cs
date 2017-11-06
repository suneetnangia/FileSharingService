using System.Net;
using System.Threading.Tasks;

namespace Microsoft.Demos.FileSharingService.API.Repositories
{
    public interface ITokenProvider
    {
        // TODO: Consider using SecureString to store token and storage account master key
        Task<string> GetReadTokenAsync(string containerName, string filePath, IPAddress sourceIPAddress);

        Task<string> GetDeleteTokenAsync(string containerName, string filePath, IPAddress sourceIPAddress);

        Task<string> GetReadDeleteTokenAsync(string containerName, string filePath, IPAddress sourceIPAddress);
    }
}