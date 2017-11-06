using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Demos.FileSharingService.API.Repositories;

namespace Microsoft.Demos.FileSharingService.API.Controllers
{
    [Route("api/[controller]")]
    public class FileMapController : Controller
    {
        private IFileMapRepository _fileMap;

        public FileMapController(IFileMapRepository fileMap) 
        {
            this._fileMap = fileMap;
        }

        // GET api/filemap/userid
        [HttpGet()]
        public async Task<IActionResult> Get([FromQuery]string authenticatedUsername)
        { 
            var fileMaps = await this._fileMap.GetFilesAsync(authenticatedUsername);

            if (fileMaps == null || fileMaps.Count == 0)
                return NotFound(authenticatedUsername);
            else
                return Ok(fileMaps);
        }
    }
}