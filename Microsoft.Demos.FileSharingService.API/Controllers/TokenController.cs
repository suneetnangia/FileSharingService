using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Demos.FileSharingService.API.Models;
using Microsoft.Demos.FileSharingService.API.Repositories;

namespace Microsoft.Demos.FileSharingService.API.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {        
        private IFileMapRepository _fileMap;
        private ITokenProvider _tokenProvider;        

        public TokenController(ITokenProvider tokenProvider, IFileMapRepository fileMap)
        {
            this._tokenProvider = tokenProvider;
            this._fileMap = fileMap;
        }

        // GET /api/token/containername/vfolder1/filename.json
        [HttpGet("{containerName}/{*filePath}")]
        public async Task<IActionResult> Get([FromQuery]string remoteIPAddress, [FromQuery]string authenticatedUsername, string containerName, string filePath)
        {
            var access = await this._fileMap.GetPermissionsAsync(authenticatedUsername, containerName, filePath);         
            
            if (access.HasFlag(FilePermissions.Read | FilePermissions.Delete))                
                return Ok(await this._tokenProvider.GetReadDeleteTokenAsync(containerName, filePath, IPAddress.Parse(remoteIPAddress)));
            if (access.HasFlag(FilePermissions.Delete))
                return Ok(await this._tokenProvider.GetDeleteTokenAsync(containerName, filePath, IPAddress.Parse(remoteIPAddress)));
            if (access.HasFlag(FilePermissions.Read))
                return Ok(await this._tokenProvider.GetReadTokenAsync(containerName, filePath, IPAddress.Parse(remoteIPAddress)));
            else
                return NotFound(filePath);
        }
    }
}