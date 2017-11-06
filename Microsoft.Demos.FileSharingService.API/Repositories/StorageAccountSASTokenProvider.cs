using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Demos.FileSharingService.API.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Microsoft.Demos.FileSharingService.API.Repositories
{
    /// <summary>
    /// Azure Storage Account SAS token provider implementation.
    /// </summary>
    public class StorageAccountSASTokenProvider : TokenProvider
    {
        private string _storageAccountConnectionString;
        // TODO: May need to replace multiple parameters for file access policies with a dictionary parameter.
        public StorageAccountSASTokenProvider(string storageAccountConnectionString, FileAccessPolicy readAccessPolicy, FileAccessPolicy deleteAccessPolicy, FileAccessPolicy readDeleteAccessPolicy) 
            : base(readAccessPolicy, deleteAccessPolicy, readDeleteAccessPolicy)
        {
            this._storageAccountConnectionString = storageAccountConnectionString;
        }

        protected override async Task<string> GetTokenAsync(string containerName, string filePath, FileAccessPolicy accessPolicy, IPAddress sourceIPAddress)
        {
            var storageAccount = CloudStorageAccount.Parse(this._storageAccountConnectionString);

            var filesContainer = storageAccount.CreateCloudBlobClient().GetContainerReference(containerName);
            var file = await filesContainer.GetBlobReferenceFromServerAsync(filePath);

            var blobPolicy =  new SharedAccessBlobPolicy()
                                {
                                    SharedAccessExpiryTime = new DateTimeOffset(DateTime.UtcNow.Add(accessPolicy.TTL)),
                                    Permissions = (SharedAccessBlobPermissions) accessPolicy.Permissions
                                };
            
            // Return the SAS token for a blob.
            return file.GetSharedAccessSignature(blobPolicy, null, null, (SharedAccessProtocol) accessPolicy.Protocol, new IPAddressOrRange(sourceIPAddress.ToString()));
        }
    }
}