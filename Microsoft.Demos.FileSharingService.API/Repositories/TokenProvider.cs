using System.Net;
using System.Threading.Tasks;

using Microsoft.Demos.FileSharingService.API.Models;

namespace Microsoft.Demos.FileSharingService.API.Repositories
{
    /// <summary>
    /// Generic base class for token provider implementations.
    /// </summary>
    public abstract class TokenProvider : ITokenProvider
    {        
        private FileAccessPolicy _readAccessPolicy;
        private FileAccessPolicy _deleteAccessPolicy;
        private FileAccessPolicy _readDeleteAccessPolicy;

        public FileAccessPolicy DeleteAccessPolicy { get; }

        protected abstract Task<string> GetTokenAsync(string containerName, string filePath, FileAccessPolicy tokenPolicy, IPAddress sourceIPAddress);

        public TokenProvider(FileAccessPolicy readAccessPolicy, FileAccessPolicy deleteAccessPolicy, FileAccessPolicy readDeleteAccessPolicy)
        {
            this._readAccessPolicy = readAccessPolicy;            
            this._deleteAccessPolicy = deleteAccessPolicy;
            this._readDeleteAccessPolicy = readDeleteAccessPolicy;
        }

        public Task<string> GetReadTokenAsync(string containerName, string filePath, IPAddress sourceIPAddress)
        {
            return this.GetTokenAsync(containerName.ToLowerInvariant(), filePath.ToLowerInvariant(), this._readAccessPolicy, sourceIPAddress);
        }

        public Task<string> GetDeleteTokenAsync(string containerName, string filePath, IPAddress sourceIPAddress)
        {
            return this.GetTokenAsync(containerName.ToLowerInvariant(), filePath.ToLowerInvariant(), this._deleteAccessPolicy, sourceIPAddress);
        }

        public Task<string> GetReadDeleteTokenAsync(string containerName, string filePath, IPAddress sourceIPAddress)
        {
            return this.GetTokenAsync(containerName.ToLowerInvariant(), filePath.ToLowerInvariant(), this._readDeleteAccessPolicy, sourceIPAddress);
        }
    }
}