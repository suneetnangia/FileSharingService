using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Demos.FileSharingService.API.Models;

namespace Microsoft.Demos.FileSharingService.API.Repositories
{
    public interface IFileMapRepository
    {
        Task<FilePermissions> GetPermissionsAsync(string identity, string containerName, string filePath);

        Task<IList<FileMap>> GetFilesAsync(string identity);
    }
}