using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Demos.FileSharingService.API.Models;

namespace Microsoft.Demos.FileSharingService.API.Repositories
{
    public class CosmosDBFileMapRepository : IFileMapRepository
    {
        private DocumentClient _client;
        private string _dbId;
        private string _collectionId;

        public CosmosDBFileMapRepository(DocumentClient client, string dbId, string collectionId)
        {
            this._client = client;
            this._dbId = dbId;
            this._collectionId = collectionId;
        }

        public async Task<FilePermissions> GetPermissionsAsync(string identity, string containerName, string filePath)
        {
            var feedOptions = new FeedOptions { MaxItemCount = 1 };
            var query = this._client.CreateDocumentQuery<FileMap>(UriFactory.CreateDocumentCollectionUri(this._dbId, this._collectionId), feedOptions)
                        .Where(f => f.Identity == identity.ToLowerInvariant() && f.File.Container == containerName.ToLowerInvariant() && f.File.Path == filePath.ToLowerInvariant())
                        .AsDocumentQuery();

            var result = await query.ExecuteNextAsync<FileMap>();

            return result.FirstOrDefault().File.Permissions;
        }

        public async Task<IList<FileMap>> GetFilesAsync(string identity)
        {
            // TODO: Make max count configurable
            var feedOptions = new FeedOptions { MaxItemCount = 1000 };
            var query = this._client.CreateDocumentQuery<FileMap>(UriFactory.CreateDocumentCollectionUri(this._dbId, this._collectionId), feedOptions)
                        .Where(f => f.Identity == identity.ToLowerInvariant())
                        .AsDocumentQuery();

            return (await query.ExecuteNextAsync<FileMap>()).ToList<FileMap>();
        }
    }
}