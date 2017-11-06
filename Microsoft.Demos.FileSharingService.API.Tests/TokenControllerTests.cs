using System;
using System.Collections.Generic;
using System.Net;
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
    public class TokenControllerTests
    {
        [TestMethod]
        public async Task TestTokenControllerReturnsValidReadToken()
        {
            // Arrange
            var userId = "suneetnangia@yxz.com";
            var containerName = "AllFiles";
            var filePath = "vfolder1/v2/ffmpeg.dll";
            var sourceIp = IPAddress.Parse("192.168.1.11");
            var token = "?sv=2016-05-31&sr=b&sig=t9M3b6gwW4KQbtvtOUMcsSAAgOaUFP2lARMk0tRtQ%2F0%3D&spr=https&sip=%3A%3A1&se=2017-10-26T12%3A33%3A39Z&sp=r";

            var tokenProviderMock = new Mock<ITokenProvider>();
            tokenProviderMock.Setup(x => x.GetReadTokenAsync(containerName, filePath, sourceIp)).Returns(Task.FromResult(token));

            var fileMapRepositoryMock = new Mock<IFileMapRepository>();
            fileMapRepositoryMock.Setup(x => x.GetPermissionsAsync(userId, containerName, filePath)).Returns(Task.FromResult(FilePermissions.Read));

            var controller = new TokenController(tokenProviderMock.Object, fileMapRepositoryMock.Object);

            // Act
            var result = await controller.Get(sourceIp.ToString(), userId, containerName, filePath);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual<string>(token, ((string)((OkObjectResult)(result)).Value));
        }

        [TestMethod]
        public async Task TestTokenControllerReturnsValidReadDeleteToken()
        {
            // Arrange
            var userId = "suneetnangia@yxz.com";
            var containerName = "AllFiles";
            var filePath = "vfolder1/v2/ffmpeg.dll";
            var sourceIp = IPAddress.Parse("192.168.1.11");
            var token = "?sv=2016-05-31&sr=b&sig=t9M3b6gwW4KQbtvtOUMcsSAAgOaUFP2lARMk0tRtQ%2F0%3D&spr=https&sip=%3A%3A1&se=2017-10-26T12%3A33%3A39Z&sp=rd";
            
            var tokenProviderMock = new Mock<ITokenProvider>();
            tokenProviderMock.Setup(x => x.GetReadDeleteTokenAsync(containerName, filePath, sourceIp)).Returns(Task.FromResult(token));

            var fileMapRepositoryMock = new Mock<IFileMapRepository>();
            fileMapRepositoryMock.Setup(x => x.GetPermissionsAsync(userId, containerName, filePath)).Returns(Task.FromResult(FilePermissions.Read | FilePermissions.Delete));

            var controller = new TokenController(tokenProviderMock.Object, fileMapRepositoryMock.Object);

            // Act
            var result = await controller.Get(sourceIp.ToString(), userId, containerName, filePath);
            
            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual<string>(token, ((string)((OkObjectResult)(result)).Value));
        }

        [TestMethod]
        public async Task TestTokenControllerReturnsValidDeleteToken()
        {
            // Arrange
            var userId = "suneetnangia@yxz.com";
            var containerName = "AllFiles";
            var filePath = "vfolder1/v2/ffmpeg.dll";
            var sourceIp = IPAddress.Parse("192.168.1.11");
            var token = "?sv=2016-05-31&sr=b&sig=t9M3b6gwW4KQbtvtOUMcsSAAgOaUFP2lARMk0tRtQ%2F0%3D&spr=https&sip=%3A%3A1&se=2017-10-26T12%3A33%3A39Z&sp=d";

            var tokenProviderMock = new Mock<ITokenProvider>();
            tokenProviderMock.Setup(x => x.GetDeleteTokenAsync(containerName, filePath, sourceIp)).Returns(Task.FromResult(token));

            var fileMapRepositoryMock = new Mock<IFileMapRepository>();
            fileMapRepositoryMock.Setup(x => x.GetPermissionsAsync(userId, containerName, filePath)).Returns(Task.FromResult(FilePermissions.Delete));

            var controller = new TokenController(tokenProviderMock.Object, fileMapRepositoryMock.Object);

            // Act
            var result = await controller.Get(sourceIp.ToString(), userId, containerName, filePath);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual<string>(token, ((string)((OkObjectResult)(result)).Value));
        }


        [TestMethod]
        public async Task TestTokenControllerReturnsNotFoundWhenAuthorisationFails()
        {
            // Arrange
            var userId = "suneetnangia@yxz.com";
            var requestUserId = string.Concat(userId, "abc");

            var containerName = "AllFiles";
            var filePath = "vfolder1/v2/ffmpeg.dll";
            var sourceIp = IPAddress.Parse("192.168.1.11");
            var token = "?sv=2016-05-31&sr=b&sig=t9M3b6gwW4KQbtvtOUMcsSAAgOaUFP2lARMk0tRtQ%2F0%3D&spr=https&sip=%3A%3A1&se=2017-10-26T12%3A33%3A39Z&sp=r";

            var fileMap1 = new FileMap();
            fileMap1.Id = Guid.NewGuid().ToString();
            fileMap1.Identity = userId;
            fileMap1.File = new File { Container = "AllFiles", Path = "vfolder1/v2/ffmpeg.dll", Permissions = FilePermissions.Read | FilePermissions.Delete };

            var fileMap2 = new FileMap();
            fileMap2.Id = Guid.NewGuid().ToString();
            fileMap2.Identity = userId;
            fileMap2.File = new File { Container = "AllFiles", Path = "vfolder4/v1/peg.txt", Permissions = FilePermissions.Read };

            var tokenProviderMock = new Mock<ITokenProvider>();
            tokenProviderMock.Setup(x => x.GetReadTokenAsync(containerName, filePath, sourceIp)).Returns(Task.FromResult(token));

            var fileMapRepositoryMock = new Mock<IFileMapRepository>();
            fileMapRepositoryMock.Setup(x => x.GetFilesAsync(userId)).Returns(Task.FromResult((IList<FileMap>)new List<FileMap>() { fileMap1, fileMap2 }));

            var controller = new TokenController(tokenProviderMock.Object, fileMapRepositoryMock.Object);

            // Act
            var result = await controller.Get(sourceIp.ToString(), requestUserId, containerName, filePath);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual<string>(filePath, ((string)((NotFoundObjectResult)(result)).Value));
        }
    }
}