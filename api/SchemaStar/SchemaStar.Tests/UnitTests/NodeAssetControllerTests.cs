using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SchemaStar.Controllers;
using SchemaStar.DataRepositories;
using SchemaStar.DTOs.Node_DTOs;
using SchemaStar.DTOs.NodeAsset_DTOs;
using SchemaStar.Exceptions;
using SchemaStar.Models;
using SchemaStar.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchemaStar.Tests.UnitTests
{
    public class NodeAssetControllerTests
    {
        private readonly NodeAssetsController _controller;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<INodeRepository> _mockNodeRepository;
        private readonly Mock<INodeAssetRepository> _mockRepository;
        private readonly Mock<ILogger<NodeAssetsController>> _mockLogger;

        public NodeAssetControllerTests()
        {
            //Mock services
            _mockRepository = new Mock<INodeAssetRepository>();
            _mockNodeRepository = new Mock<INodeRepository>();
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<NodeAssetsController>>();

            //Initialize controller
            _controller = new NodeAssetsController(
                _mockUserService.Object,
                _mockLogger.Object,
                _mockRepository.Object,
                _mockNodeRepository.Object
            );
        }

        [Fact]
        public async Task GetNodeAssets_WhenUserIdNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var nodePublicId = Guid.NewGuid();

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);
            //Act and Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.GetNodeAssets(nodePublicId));
        }

        [Fact]
        public async Task GetNodeAssets_WhenUserIdIsValid_ReturnsListOfNodeAssetResponseDTOs()
        {
            //Arrange
            var nodePublicId = Guid.NewGuid();
            var userId = 1UL;
            var listOfNodeAssets = new List<NodeAsset> {
                new NodeAsset { PublicId = Guid.NewGuid().ToMySqlBinary(), NodeAssetName = "NodeAsset 1" },
                new NodeAsset { PublicId = Guid.NewGuid().ToMySqlBinary(), NodeAssetName = "NodeAsset 2" }
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodeAssetsByNodeIdAsync(nodePublicId.ToMySqlBinary(), userId)).ReturnsAsync(listOfNodeAssets);

            //Act and Assert
            var result = await _controller.GetNodeAssets(nodePublicId);

            var response = Assert.IsType<List<NodeAssetResponseDTO>>(result.Value);
            Assert.Equal(2, response.Count());
            Assert.Equal("NodeAsset 1", response[0].NodeAssetName);
        }

        [Fact]
        public async Task GetNodeAsset_WhenUserIdNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var publicId = Guid.NewGuid();

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);
            //Act and Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.GetNodeAsset(publicId));
        }

        [Fact]
        public async Task GetNodeAsset_WhenNodeAssetDoesNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodeAssetByPublicIdAsync(It.IsAny<byte[]>(), userId))
                .ReturnsAsync((NodeAsset)null);

            //Act and Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetNodeAsset(publicId));
        }

        [Fact]
        public async Task GetNodeAsset_WhenGettingValidNodeAsset_ReturnsNodeAssetResponseDTO()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var existingNodeAsset = new NodeAsset
            {
                PublicId = publicId.ToMySqlBinary(),
                NodeAssetName = "Test_NodeAsset",
                FileSize = 10
            };

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodeAssetByPublicIdAsync(It.IsAny<byte[]>(), userId))
                .ReturnsAsync(existingNodeAsset);

            //Act and Assert
            var result = await _controller.GetNodeAsset(publicId);

            var response = Assert.IsType<NodeAssetResponseDTO>(result.Value);

            Assert.Equal(publicId, response.PublicId);
            Assert.Equal("Test_NodeAsset", response.NodeAssetName);
            Assert.Equal(10, response.FileSize);
        }

        [Fact]
        public async Task UpdateNodeAsset_WhenUserIdIsNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var publicId = Guid.NewGuid();
            var request = new NodeAssetUpdateRequestDTO
            {
                NodeAssetName = "Updated NodeAsset",
                FileSize = 15
            };
            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);

            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.UpdateNodeAsset(publicId, request));
        }


        [Fact]
        public async Task UpdateNodeAsset_WhenNodeAssetDoesNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var request = new NodeAssetUpdateRequestDTO
            {
                NodeAssetName = "Updated NodeAsset",
                FileSize = 15
            };
            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodeAssetByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync((NodeAsset)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _controller.UpdateNodeAsset(publicId, request));
        }


        [Fact]
        public async Task UpdateNodeAsset_WhenNodeAssetUpdateSucceeds_ReturnsNoContent()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var existingNodeAsset = new NodeAsset
            {
                PublicId = publicId.ToMySqlBinary(),
                NodeAssetName = "Test_NodeAsset",
                FileSize = 15,
                NodeAssetType = Models.Enums.NodeAssetEnums.NodeAssetType.Image
            };

            var request = new NodeAssetUpdateRequestDTO
            {
                NodeAssetName = "Updated NodeAsset",
                FileSize = 25,
                NodeAssetType = Models.Enums.NodeAssetEnums.NodeAssetType.Video

            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodeAssetByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(existingNodeAsset);

            //Act and assert
            var result = await _controller.UpdateNodeAsset(publicId, request);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal("Updated NodeAsset", existingNodeAsset.NodeAssetName);
            Assert.Equal(25, existingNodeAsset.FileSize);
            Assert.Equal(Models.Enums.NodeAssetEnums.NodeAssetType.Video, existingNodeAsset.NodeAssetType);
            //Verify
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateNodeAsset_WhenOnlyNameIsProvided_DoesNotOverwriteOtherFields()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var existingNodeAsset = new NodeAsset
            {
                PublicId = publicId.ToMySqlBinary(),
                NodeAssetName = "Test_NodeAsset",
                FileSize = 100
            };

            var request = new NodeAssetUpdateRequestDTO
            {
                NodeAssetName = "Updated NodeAsset",
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodeAssetByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(existingNodeAsset);

            //Act and assert
            var result = await _controller.UpdateNodeAsset(publicId, request);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal("Updated NodeAsset", existingNodeAsset.NodeAssetName);
            Assert.Equal(100, existingNodeAsset.FileSize);
            //Verify
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task PostNodeAsset_WhenUserIdIsNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var request = new NodeAssetRequestDTO
            {
                NodeAssetName = "New NodeAsset",
                FileSize = 50,
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);

            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.PostNodeAsset(request));
        }


        [Fact]
        public async Task PostNodeAsset_WhenNodeIdIsNull_ThrowsNotFoundException()
        {
            //Arrange
            var nodePublicId = Guid.NewGuid();
            var userId = 1UL;
            var request = new NodeAssetRequestDTO
            {
                NodeAssetName = "New NodeAsset",
                FileSize = 50,
                NodeId = nodePublicId,
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockNodeRepository.Setup(r => r.GetInternalIdByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync((ulong?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _controller.PostNodeAsset(request));
        }


        [Fact]
        public async Task PostNodeAsset_WhenNodeAssetCreationSucceeds_Returns201CreationActionResult()
        {
            //Arrange
            var nodeAssetPublicId = Guid.NewGuid();
            var nodeId = 1UL;

            var userId = 1UL;
            var request = new NodeAssetRequestDTO
            {
                NodeAssetName = "New NodeAsset",
                FileSize = 50,
                NodeAssetSource = Models.Enums.NodeAssetEnums.NodeAssetSource.Upload
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockNodeRepository.Setup(r => r.GetInternalIdByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(nodeId);

            //Act and Assert
            var result = await _controller.PostNodeAsset(request);

            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var response = Assert.IsType<NodeAssetResponseDTO>(createdAtResult.Value);

            Assert.Equal(201, createdAtResult.StatusCode);
            Assert.Equal(request.NodeAssetName, response.NodeAssetName);
            Assert.Equal(request.FileSize, response.FileSize);
            Assert.Equal(request.NodeAssetSource, response.NodeAssetSource);
            Assert.NotEqual(Guid.Empty, response.PublicId);

            //Verify
            _mockRepository.Verify(r => r.Add(It.Is<NodeAsset>(n => n.NodeId == nodeId)), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteNodeAsset_WhenUserIdIsNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var publicId = Guid.NewGuid();

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);

            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.DeleteNodeAsset(publicId));
        }

        [Fact]
        public async Task DeleteNodeAsset_WhenNodeAssetIsNull_ThrowsNotFoundException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodeAssetByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync((NodeAsset)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _controller.DeleteNodeAsset(publicId));
        }


        [Fact]
        public async Task DeleteNodeAsset_WhenNodeAssetDeletionSucceeds_ReturnsNoContent()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var existingNodeAsset = new NodeAsset
            {
                PublicId = publicId.ToMySqlBinary(),
                NodeAssetName = "Test_NodeAsset",
                FileSize = 100
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodeAssetByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(existingNodeAsset);

            //Act and Assert
            var result = await _controller.DeleteNodeAsset(publicId);
            Assert.IsType<NoContentResult>(result);

            //Verify
            _mockRepository.Verify(r => r.Delete(existingNodeAsset), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
