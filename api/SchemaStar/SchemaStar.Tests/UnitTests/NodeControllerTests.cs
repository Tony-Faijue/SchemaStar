using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SchemaStar.Controllers;
using SchemaStar.DataRepositories;
using SchemaStar.DTOs.Node_DTOs;
using SchemaStar.DTOs.Nodeweb_DTOs;
using SchemaStar.Exceptions;
using SchemaStar.Models;
using SchemaStar.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchemaStar.Tests.UnitTests
{
    public class NodeControllerTests
    {
        private readonly NodesController _controller;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<INodewebRepository> _mockNodewebRepository;
        private readonly Mock<INodeRepository> _mockRepository;
        private readonly Mock<ILogger<NodesController>> _mockLogger;

        public NodeControllerTests()
        {
            //Mock services
            _mockRepository = new Mock<INodeRepository>();
            _mockNodewebRepository = new Mock<INodewebRepository>();
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<NodesController>>();

            //Initialize controller
            _controller = new NodesController(
                _mockUserService.Object,
                _mockLogger.Object,
                _mockRepository.Object,
                _mockNodewebRepository.Object
            );
        }

        [Fact]
        public async Task GetNodes_WhenUserIdNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var nodeWebPublicId = Guid.NewGuid();

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);
            //Act and Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.GetNodes(nodeWebPublicId));
        }

        [Fact]
        public async Task GetNodes_WhenUserIdIsValid_ReturnsListOfNodeResponseDTOs()
        {
            //Arrange
            var nodeWebPublicId = Guid.NewGuid();
            var nodeWeb = new Nodeweb { PublicId = nodeWebPublicId.ToMySqlBinary() };

            var userId = 1UL;
            var listOfNodes = new List<Node> {
                new Node { PublicId = Guid.NewGuid().ToMySqlBinary(), NodeName = "Node 1", NodeWeb = nodeWeb },
                new Node { PublicId = Guid.NewGuid().ToMySqlBinary(), NodeName = "Node 2", NodeWeb = nodeWeb }
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodesByNodeWebIdAsync(nodeWebPublicId.ToMySqlBinary(), userId)).ReturnsAsync(listOfNodes);

            //Act and Assert
            var result = await _controller.GetNodes(nodeWebPublicId);

            var response = Assert.IsType<List<NodeResponseDTO>>(result.Value);
            Assert.Equal("Node 1", response[0].NodeName);
            Assert.Equal(2, response.Count());
            Assert.Equal(nodeWebPublicId, response[0].NodeWebId);
        }

        [Fact]
        public async Task GetNode_WhenUserIdNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var publicId = Guid.NewGuid();

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);
            //Act and Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.GetNode(publicId));
        }

        [Fact]
        public async Task GetNode_WhenNodeDoesNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodeByPublicIdAsync(It.IsAny<byte[]>(), userId))
                .ReturnsAsync((Node)null);

            //Act and Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetNode(publicId));
        }

        [Fact]
        public async Task GetNode_WhenGettingValidNode_ReturnsNodeResponseDTO()
        {
            //Arrange
            var nodeWebPublicId = Guid.NewGuid();
            var nodeWeb = new Nodeweb { PublicId = nodeWebPublicId.ToMySqlBinary() };

            var userId = 1UL;
            var publicId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var existingNode = new Node
            {
                PublicId = publicId.ToMySqlBinary(),
                NodeName = "Test_Node",
                CreatedAt = now,
                NodeWeb = nodeWeb
            };

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodeByPublicIdAsync(It.IsAny<byte[]>(), userId))
                .ReturnsAsync(existingNode);

            //Act and Assert
            var result = await _controller.GetNode(publicId);

            var response = Assert.IsType<NodeResponseDTO>(result.Value);

            Assert.Equal(publicId, response.PublicId);
            Assert.Equal("Test_Node", response.NodeName);
            Assert.Equal(now, response.CreatedAt);
            Assert.Equal(nodeWebPublicId, response.NodeWebId);
        }

        [Fact]
        public async Task UpdateNode_WhenUserIdIsNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var publicId = Guid.NewGuid();
            var request = new NodeUpdateRequestDTO
            {
                NodeName = "Updated Node",
                NodeDescription = "Node is Updated"
            };
            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);

            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.UpdateNode(publicId, request));
        }

        [Fact]
        public async Task UpdateNode_WhenNodeDoesNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var request = new NodeUpdateRequestDTO
            {
                NodeName = "Updated Node",
                NodeDescription = "Node is Updated"
            };
            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodeByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync((Node)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _controller.UpdateNode(publicId, request));
        }

        [Fact]
        public async Task UpdateNode_WhenNodeUpdateSucceeds_ReturnsNoContent()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var existingNode = new Node
            {
                PublicId = publicId.ToMySqlBinary(),
                NodeName = "Test_Node",
                NodeDescription = "My Node",
                CreatedAt = now,
            };

            var request = new NodeUpdateRequestDTO
            {
                NodeName = "Updated Node",
                NodeDescription = "My Updated Node"
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodeByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(existingNode);

            //Act and assert
            var result = await _controller.UpdateNode(publicId, request);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal("Updated Node", existingNode.NodeName);
            Assert.Equal("My Updated Node", existingNode.NodeDescription);
            //Verify
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateNode_WhenOnlyNameIsProvided_DoesNotOverwriteOtherFields()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var existingNode = new Node
            {
                PublicId = publicId.ToMySqlBinary(),
                NodeName = "Test_Node",
                NodeDescription = "Keep Description",
                PositionX = 100
            };

            var request = new NodeUpdateRequestDTO
            {
                NodeName = "Updated Node",
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodeByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(existingNode);

            //Act and assert
            var result = await _controller.UpdateNode(publicId, request);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal("Updated Node", existingNode.NodeName);
            Assert.Equal("Keep Description", existingNode.NodeDescription);
            Assert.Equal(100, existingNode.PositionX);
            //Verify
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task PostNode_WhenUserIdIsNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var request = new NodeRequestDTO
            {
                NodeName = "New Node",
                PositionY = 200,
                Height = 50,
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);

            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.PostNode(request));
        }

        [Fact]
        public async Task PostNode_WhenNodeWebIdIsNull_ThrowsNotFoundException()
        {
            //Arrange
            var nodeWebpublicId = Guid.NewGuid();
            var userId = 1UL;
            var request = new NodeRequestDTO
            {
                NodeName = "New Node",
                PositionY = 200,
                Height = 50,
                NodeWebId = nodeWebpublicId,
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockNodewebRepository.Setup(r => r.GetInternalIdByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync((ulong?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _controller.PostNode(request));
        }

        [Fact]
        public async Task PostNode_WhenNodeCreationSucceeds_Returns201CreationActionResult()
        {
            //Arrange
            var nodeWebPublicId = Guid.NewGuid();
            var nodeWebId = 1UL;

            var nodeWeb = new Nodeweb { Id = nodeWebId, PublicId = nodeWebPublicId.ToMySqlBinary() };

            var userId = 1UL;
            var request = new NodeRequestDTO
            {
                NodeName = "New Node",
                PositionY = 200,
                Height = 50,
                NodeWebId = nodeWebPublicId
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockNodewebRepository.Setup(r => r.GetInternalIdByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(nodeWebId);

            //Act and Assert
            var result = await _controller.PostNode(request);

            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var response = Assert.IsType<NodeResponseDTO>(createdAtResult.Value);

            Assert.Equal(201, createdAtResult.StatusCode);
            Assert.Equal(request.NodeName, response.NodeName);
            Assert.Equal(request.PositionY, response.PositionY);
            Assert.Equal(request.Height, response.Height);
            Assert.Equal(nodeWebPublicId, response.NodeWebId);

            //Verify
            _mockRepository.Verify(r => r.Add(It.Is<Node>(n => n.NodeWebId == nodeWebId)), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteNode_WhenUserIdIsNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var publicId = Guid.NewGuid();

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);

            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.DeleteNode(publicId));
        }

        [Fact]
        public async Task DeleteNode_WhenNodeIsNull_ThrowsNotFoundException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodeByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync((Node)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _controller.DeleteNode(publicId));
        }

        [Fact]
        public async Task DeleteNode_WhenNodeDeletionSucceeds_ReturnsNoContent()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var existingNode = new Node
            {
                PublicId = publicId.ToMySqlBinary(),
                NodeName = "Test_Node",
                NodeDescription = "My Description",
                PositionX = 100
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodeByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(existingNode);

            //Act and Assert
            var result = await _controller.DeleteNode(publicId);
            Assert.IsType<NoContentResult>(result);

            //Verify
            _mockRepository.Verify(r => r.Delete(existingNode), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetNodeFull_WhenUserIdNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var publicId = Guid.NewGuid();
            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);
            //Act and Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.GetNodeFull(publicId));
        }


        [Fact]
        public async Task GetNodeFull_WhenNodeDoesNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetFullNodeByPublicIdAsync(It.IsAny<byte[]>(), userId))
                .ReturnsAsync((Node)null);

            //Act and Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetNodeFull(publicId));
        }

        [Fact]
        public async Task GetNodeFull_WhenGettingValidNode_ReturnsNodeFullResponseDTO()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var existingNode = new Node
            {
                PublicId = publicId.ToMySqlBinary(),
                NodeName = "Test_Node"
            };

            var nodeAsset1 = new NodeAsset { PublicId = Guid.NewGuid().ToMySqlBinary(), NodeAssetName = "NodeAsset 1" };
            var nodeAsset2 = new NodeAsset { PublicId = Guid.NewGuid().ToMySqlBinary(), NodeAssetName = "NodeAsset 2" };

            existingNode.NodeAssets = new List<NodeAsset> { nodeAsset1, nodeAsset2 };
            

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetFullNodeByPublicIdAsync(It.IsAny<byte[]>(), userId))
                .ReturnsAsync(existingNode);

            //Act and Assert
            var result = await _controller.GetNodeFull(publicId);

            var response = Assert.IsType<NodeFullResponseDTO>(result.Value);

            Assert.Equal(publicId, response.PublicId);
            Assert.Equal("Test_Node", response.NodeName);
            //Verify NodeAssets
            Assert.Equal(2, response.NodeAssets.Count);
            Assert.Equal(nodeAsset1.PublicId.ToGuidFromMySqlBinary(), response.NodeAssets[0].PublicId);
            Assert.Equal("NodeAsset 2", response.NodeAssets[1].NodeAssetName);
        }

        [Fact]
        public async Task BulkUpdateNodes_WhenUserIdNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var nodewebId = Guid.NewGuid();

            var nodeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            var request = new List<NodeBulkUpdateRequestDTO>
            {
                new NodeBulkUpdateRequestDTO { PublicId = nodeIds[0], NodeName = "Updated 1"},
                new NodeBulkUpdateRequestDTO { PublicId = nodeIds[1], NodeName = "Updated 2"}
            };

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);
            //Act and Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.BulkUpdateNodes(nodewebId, request));
        }

        [Fact]
        public async Task BulkUpdateNodes_WhenUpdateSucceeds_ReturnsNoContent()
        {
            //Arrange
            var userId = 1UL;
            var nodewebId = Guid.NewGuid();
            var nodeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            var request = new List<NodeBulkUpdateRequestDTO>
            {
                new NodeBulkUpdateRequestDTO { PublicId = nodeIds[0], NodeName = "Updated 1"},
                new NodeBulkUpdateRequestDTO { PublicId = nodeIds[1], NodeName = "Updated 2"}
            };

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)userId);
            //Act and Assert
            _mockRepository.Setup(r => r.UpdateNodesBulkAsync(It.IsAny<IEnumerable<Node>>(), It.IsAny<byte[]>(), userId))
                .Returns(Task.CompletedTask);

            var result = await _controller.BulkUpdateNodes(nodewebId, request);

            Assert.IsType<NoContentResult>(result);

            _mockRepository.Verify(r => r.UpdateNodesBulkAsync(
                It.Is<IEnumerable<Node>>(list => list.Count() == 2),
                It.IsAny<byte[]>(),
                userId),
                Times.Once
             );
        }

        [Fact]
        public async Task BulkDeleteNodes_WhenUserIdNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var userId = 1UL;
            var nodewebId = Guid.NewGuid();
            var nodeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);
            //Act and Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.BulkDeleteNodes(nodewebId, nodeIds));
        }

        [Fact]
        public async Task BulkDeleteNodes_WhenListIsEmpty_ThrowsArgumentException()
        {
            //Arrange
            var userId = 1UL;
            var nodewebId = Guid.NewGuid();
            var nodeIds = new List<Guid> { };

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)userId);
            //Act and Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _controller.BulkDeleteNodes(nodewebId, nodeIds));
        }

        [Fact]
        public async Task BulkDeleteNodes_WhenDeleteSucceeds_ReturnsNoContent()
        {
            //Arrange
            var userId = 1UL;
            var nodewebId = Guid.NewGuid();
            var nodeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)userId);
            //Act and Assert

            //return 2 which is the number of rows deleted
            _mockRepository.Setup(r => r.DeleteNodesBulkAsync(It.IsAny<IEnumerable<byte[]>>(), It.IsAny<byte[]>(), userId))
                .ReturnsAsync(2);

            var result = await _controller.BulkDeleteNodes(nodewebId, nodeIds);

            Assert.IsType<NoContentResult>(result);

            _mockRepository.Verify(r => r.DeleteNodesBulkAsync(
                It.Is<IEnumerable<byte[]>>(ids => ids.Count() == 2),
                It.IsAny<byte[]>(),
                userId),
                Times.Once
             );
        }
    }
}
