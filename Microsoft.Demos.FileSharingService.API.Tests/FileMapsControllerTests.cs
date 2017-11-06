using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Demos.FileSharingService.API.Controllers;
using Microsoft.Demos.FileSharingService.API.Models;
using Microsoft.Demos.FileSharingService.API.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Demos.FileSharingService.API.Tests
{
    /// <summary>
    /// For asp.net core unit testing, please refer to documentation here https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/testing
    /// </summary>
    [TestClass]
    public class FileMapControllerTests
    {
        [TestMethod]
        public async Task TestFileMapControllerReturnsUserFileMaps()
        {
            // Arrange
            var userId = "suneetnangia@yxz.com";

            var fileMap1 = new FileMap();
            fileMap1.Id = Guid.NewGuid().ToString();
            fileMap1.Identity = userId;
            fileMap1.File = new File { Container = "AllFiles", Path = "vfolder1/v2/ffmpeg.dll", Permissions = FilePermissions.Read | FilePermissions.Delete };

            var fileMap2 = new FileMap();
            fileMap2.Id = Guid.NewGuid().ToString();
            fileMap2.Identity = userId;
            fileMap2.File = new File { Container = "AllFiles", Path = "vfolder4/v1/peg.txt", Permissions = FilePermissions.Read };

            var fileMapRepositoryMock = new Mock<IFileMapRepository>();
            fileMapRepositoryMock.Setup(x => x.GetFilesAsync(userId)).Returns(Task.FromResult((IList<FileMap>)new List<FileMap>() { fileMap1, fileMap2 }));

            var controller = new FileMapController(fileMapRepositoryMock.Object);

            // Act
            var result = await controller.Get(userId);
            
            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual<int>(2, ((IList<FileMap>)((OkObjectResult)(result)).Value).Count);
            
            // TODO: Compate object values
        }

        [TestMethod]
        public async Task TestFileMapControllerDoesNotReturnUserFileMaps()
        {
            // Arrange
            var userId = "suneetnangia@yxz.com";
            var requestUserId = string.Concat(userId,"abc");

            var fileMap1 = new FileMap();
            fileMap1.Id = Guid.NewGuid().ToString();
            fileMap1.Identity = userId;
            fileMap1.File = new File { Container = "AllFiles", Path = "vfolder1/v2/ffmpeg.dll", Permissions = FilePermissions.Read | FilePermissions.Delete };

            var fileMap2 = new FileMap();
            fileMap2.Id = Guid.NewGuid().ToString();
            fileMap2.Identity = userId;
            fileMap2.File = new File { Container = "AllFiles", Path = "vfolder4/v1/peg.txt", Permissions = FilePermissions.Read };

            var fileMapRepositoryMock = new Mock<IFileMapRepository>();
            fileMapRepositoryMock.Setup(x => x.GetFilesAsync(userId)).Returns(Task.FromResult((IList<FileMap>)new List<FileMap>() { fileMap1, fileMap2 }));

            var controller = new FileMapController(fileMapRepositoryMock.Object);

            // Act
            var result = await controller.Get(requestUserId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual<string>(requestUserId, ((string)(((NotFoundObjectResult)result).Value)));
        }
    }
}