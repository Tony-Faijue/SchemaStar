using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SchemaStar.Controllers;
using SchemaStar.DataRepositories;
using SchemaStar.DTOs;
using SchemaStar.DTOs.Edge_DTOs;
using SchemaStar.DTOs.NodeAsset_DTOs;
using SchemaStar.Exceptions;
using SchemaStar.Models;
using SchemaStar.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchemaStar.Tests.UnitTests
{
    public class EdgeControllerTests
    {
        private readonly EdgesController _controller;
        private readonly Mock<ILogger<EdgesController>> _mockLogger;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IEdgeRepository> _mockRepository;
        private readonly Mock<INodewebRepository> _mockNodewebRepository;
        private readonly Mock<INodeRepository> _mockNodeRepository;

        public EdgeControllerTests()
        {
            //Mock services
            _mockRepository = new Mock<IEdgeRepository>();
            _mockNodewebRepository = new Mock<INodewebRepository>();
            _mockNodeRepository = new Mock<INodeRepository>();
            _mockLogger = new Mock<ILogger<EdgesController>>();
            _mockUserService = new Mock<IUserService>();

            //Initialize controller
            _controller = new EdgesController(
                _mockUserService.Object,
                _mockLogger.Object,
                _mockRepository.Object,
                _mockNodewebRepository.Object,
                _mockNodeRepository.Object
            );
        }

        [Fact]
        public async Task GetNodeEdges_WhenUserIdNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var nodePublicId = Guid.NewGuid();

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);
            //Act and Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.GetEdges(nodePublicId));
        }

        [Fact]
        public async Task GetEdges_WhenUserIdIsValid_ReturnsListOfEdgeResponseDTOs()
        {
            //Arrange
            var nodewebPublicId = Guid.NewGuid();
            var userId = 1UL;
            var nodeWeb = new Nodeweb { PublicId = nodewebPublicId.ToMySqlBinary() };

            var fromNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };
            var toNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };

            var listOfEdges = new List<Edge> {
                new Edge { 
                    PublicId = Guid.NewGuid().ToMySqlBinary(), 
                    EdgeType = Models.Enums.EdgeType.Undirected,
                    FromNode = fromNode,
                    ToNode = toNode,
                    Nodeweb = nodeWeb
                },
                new Edge { 
                    PublicId = Guid.NewGuid().ToMySqlBinary(),
                    FromNode = fromNode,
                    ToNode = toNode,
                    Nodeweb = nodeWeb
                }
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetEdgesByNodeWebIdAsync(nodewebPublicId.ToMySqlBinary(), userId)).ReturnsAsync(listOfEdges);

            //Act and Assert
            var result = await _controller.GetEdges(nodewebPublicId);

            var response = Assert.IsType<List<EdgeResponseDTO>>(result.Value);
            Assert.Equal(2, response.Count());
            Assert.Equal(Models.Enums.EdgeType.Undirected, response[0].EdgeType);
            Assert.Equal(fromNode.PublicId.ToGuidFromMySqlBinary(), response[1].FromNodeId);
            Assert.Equal(nodewebPublicId, response[0].NodeWebId);
        }

        [Fact]
        public async Task GetEdge_WhenUserIdNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var publicId = Guid.NewGuid();

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);
            //Act and Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.GetEdge(publicId));
        }

        [Fact]
        public async Task GetEdge_WhenEdgeDoesNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetEdgeByPublicIdAsync(It.IsAny<byte[]>(), userId))
                .ReturnsAsync((Edge)null);

            //Act and Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetEdge(publicId));
        }

        [Fact]
        public async Task GetEdge_WhenGettingValidNodeAsset_ReturnsEdgeResponseDTO()
        {
            //Arrange
            var nodeWebPublicId = Guid.NewGuid();
            var nodeWeb = new Nodeweb { PublicId = nodeWebPublicId.ToMySqlBinary() };

            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var fromNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };
            var toNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };

            var existingEdge = new Edge
            {
                PublicId = publicId.ToMySqlBinary(),
                FromNode = fromNode,
                ToNode = toNode,
                EdgeType = Models.Enums.EdgeType.Undirected,
                Nodeweb = nodeWeb
            };

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetEdgeByPublicIdAsync(It.IsAny<byte[]>(), userId))
                .ReturnsAsync(existingEdge);

            //Act and Assert
            var result = await _controller.GetEdge(publicId);

            var response = Assert.IsType<EdgeResponseDTO>(result.Value);

            Assert.Equal(publicId, response.PublicId);
            Assert.Equal(Models.Enums.EdgeType.Undirected, response.EdgeType);
            Assert.Equal(fromNode.PublicId.ToGuidFromMySqlBinary(), response.FromNodeId);
            Assert.Equal(nodeWebPublicId, response.NodeWebId);
        }

        [Fact]
        public async Task UpdateEdge_WhenUserIdIsNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var publicId = Guid.NewGuid();
            var request = new EdgeUpdateRequestDTO
            {
                EdgeType = Models.Enums.EdgeType.Undirected
            };
            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);

            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.UpdateEdge(publicId, request));
        }

        [Fact]
        public async Task UpdateEdge_WhenEdgeDoesNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var request = new EdgeUpdateRequestDTO
            {
                EdgeType = Models.Enums.EdgeType.Undirected
            };
            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetEdgeByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync((Edge)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _controller.UpdateEdge(publicId, request));
        }

        [Fact]
        public async Task UpdateEdge_WhenFromNodeDoesNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var fromNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };
            var toNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };

            //Request node ids
            var fromNodeId = Guid.Empty;
            var toNodeId = Guid.NewGuid();

            var existingEdge = new Edge
            {
                PublicId = publicId.ToMySqlBinary(),
                FromNode = fromNode,
                ToNode = toNode,
                EdgeType = Models.Enums.EdgeType.Directed
            };

            var request = new EdgeUpdateRequestDTO
            {
                EdgeType = Models.Enums.EdgeType.Undirected,
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
            };
            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetEdgeByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(existingEdge);

            _mockNodeRepository.Setup(n => n.GetInternalIdByPublicIdAsync(fromNodeId.ToMySqlBinary(), userId)).ReturnsAsync((ulong?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _controller.UpdateEdge(publicId, request));
        }

        [Fact]
        public async Task UpdateEdge_WhenToNodeDoesNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var fromNodeUL = 1UL;

            var fromNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };
            var toNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };

            //Request node ids
            var fromNodeId = Guid.NewGuid();
            var toNodeId = Guid.Empty;

            var existingEdge = new Edge
            {
                PublicId = publicId.ToMySqlBinary(),
                FromNode = fromNode,
                ToNode = toNode,
                EdgeType = Models.Enums.EdgeType.Directed
            };

            var request = new EdgeUpdateRequestDTO
            {
                EdgeType = Models.Enums.EdgeType.Undirected,
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
            };
            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetEdgeByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(existingEdge);

            _mockNodeRepository.Setup(n => n.GetInternalIdByPublicIdAsync(fromNodeId.ToMySqlBinary(), userId)).ReturnsAsync(fromNodeUL);

            _mockNodeRepository.Setup(n => n.GetInternalIdByPublicIdAsync(toNodeId.ToMySqlBinary(), userId)).ReturnsAsync((ulong?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _controller.UpdateEdge(publicId, request));
        }

        [Fact]
        public async Task UpdateEdge_WhenSelfLoopingNodeOccurs_ThrowsValidationException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var fromNodeUL = 1UL;
            var toNodeUL = 1UL;

            var fromNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };
            var toNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };

            //Request node ids
            //Self looping node/ the same node
            var fromNodeId = Guid.NewGuid();
            var toNodeId = fromNodeId;

            var existingEdge = new Edge
            {
                PublicId = publicId.ToMySqlBinary(),
                FromNode = fromNode,
                ToNode = toNode,
                EdgeType = Models.Enums.EdgeType.Directed
            };

            var request = new EdgeUpdateRequestDTO
            {
                EdgeType = Models.Enums.EdgeType.Undirected,
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
            };
            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetEdgeByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(existingEdge);

            _mockNodeRepository.Setup(n => n.GetInternalIdByPublicIdAsync(fromNodeId.ToMySqlBinary(), userId)).ReturnsAsync(fromNodeUL);

            _mockNodeRepository.Setup(n => n.GetInternalIdByPublicIdAsync(toNodeId.ToMySqlBinary(), userId)).ReturnsAsync(toNodeUL);

            await Assert.ThrowsAsync<ValidationException>(() => _controller.UpdateEdge(publicId, request));
        }

        [Fact]
        public async Task UpdateEdge_WhenEdgeUpdateSucceds_ReturnsNoContent()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var fromNodeUL = 1UL;
            var toNodeUL = 2UL;

            var fromNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };
            var toNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };

            //Request node ids
            var fromNodeId = Guid.NewGuid();
            var toNodeId = Guid.NewGuid();

            var existingEdge = new Edge
            {
                PublicId = publicId.ToMySqlBinary(),
                FromNode = fromNode,
                ToNode = toNode,
                EdgeType = Models.Enums.EdgeType.Directed
            };

            var request = new EdgeUpdateRequestDTO
            {
                EdgeType = Models.Enums.EdgeType.Undirected,
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
            };
            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetEdgeByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(existingEdge);

            _mockNodeRepository.Setup(n => n.GetInternalIdByPublicIdAsync(fromNodeId.ToMySqlBinary(), userId)).ReturnsAsync(fromNodeUL);

            _mockNodeRepository.Setup(n => n.GetInternalIdByPublicIdAsync(toNodeId.ToMySqlBinary(), userId)).ReturnsAsync(toNodeUL);

            //Act and assert
            var result = await _controller.UpdateEdge(publicId, request);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(Models.Enums.EdgeType.Undirected, existingEdge.EdgeType);
            Assert.Equal(fromNodeUL, existingEdge.FromNodeId);
            Assert.Equal(toNodeUL, existingEdge.ToNodeId);
            //Verify
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task PostEdge_WhenUserIdIsNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var request = new EdgeRequestDTO
            {
                EdgeType = Models.Enums.EdgeType.Directed,
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);

            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.PostEdge(request));
        }

        [Fact]
        public async Task PostEdge_WhenNodewebIdIsNull_ThrowsNotFoundException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var fromNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };
            var toNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };

            //Request node ids
            var fromNodeId = Guid.NewGuid();
            var toNodeId = Guid.NewGuid();

            var request = new EdgeRequestDTO
            {
                EdgeType = Models.Enums.EdgeType.Undirected,
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockNodeRepository.Setup(r => r.GetInternalIdByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(userId);

            await Assert.ThrowsAsync<NotFoundException>(() => _controller.PostEdge(request));
        }

        [Fact]
        public async Task PostEdge_WhenFromNodeIdIsNull_ThrowsNotFoundException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var fromNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };
            var toNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };

            //Request node ids
            var fromNodeId = Guid.Empty;
            var toNodeId = Guid.NewGuid();

            var request = new EdgeRequestDTO
            {
                EdgeType = Models.Enums.EdgeType.Undirected,
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockNodeRepository.Setup(r => r.GetInternalIdByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(userId);

            _mockNodeRepository.Setup(n => n.GetInternalIdByPublicIdAsync(fromNodeId.ToMySqlBinary(), userId)).ReturnsAsync((ulong?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _controller.PostEdge(request));
        }

        [Fact]
        public async Task PostEdge_WhenToNodeIdIsNull_ThrowsNotFoundException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var fromNodeUL = 1UL;

            var fromNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };
            var toNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };

            //Request node ids
            var fromNodeId = Guid.NewGuid();
            var toNodeId = Guid.Empty;

            var request = new EdgeRequestDTO
            {
                EdgeType = Models.Enums.EdgeType.Undirected,
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockNodeRepository.Setup(r => r.GetInternalIdByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(userId);

            _mockNodeRepository.Setup(n => n.GetInternalIdByPublicIdAsync(fromNodeId.ToMySqlBinary(), userId)).ReturnsAsync(fromNodeUL);
            
            _mockNodeRepository.Setup(n => n.GetInternalIdByPublicIdAsync(toNodeId.ToMySqlBinary(), userId)).ReturnsAsync((ulong?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _controller.PostEdge(request));
        }


        [Fact]
        public async Task PostEdge_WhenEdgeSucceeds_Returns201CreationActionResult()
        {
            //Arrange
            var nodewebPublicId = Guid.NewGuid();
            var nodewebId = 5UL;

            var nodeWeb = new Nodeweb { Id = nodewebId, PublicId = nodewebPublicId.ToMySqlBinary() }; 

            var userId = 1UL;

            var fromNodeUL = 1UL;
            var toNodeUL = 2UL;

            //Request node ids
            var fromNodeId = Guid.NewGuid();
            var toNodeId = Guid.NewGuid();

            var request = new EdgeRequestDTO
            {
                EdgeType = Models.Enums.EdgeType.Undirected,
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
                NodeWebId = nodewebPublicId,
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockNodewebRepository.Setup(r => r.GetInternalIdByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(userId);

            _mockNodeRepository.Setup(n => n.GetInternalIdByPublicIdAsync(fromNodeId.ToMySqlBinary(), userId)).ReturnsAsync(fromNodeUL);

            _mockNodeRepository.Setup(n => n.GetInternalIdByPublicIdAsync(toNodeId.ToMySqlBinary(), userId)).ReturnsAsync(toNodeUL);

            //Act and assert
            var result = await _controller.PostEdge(request);

            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var response = Assert.IsType<EdgeResponseDTO>(createdAtResult.Value);

            response.FromNodeId = fromNodeId;
            response.ToNodeId = toNodeId;

            Assert.Equal(201, createdAtResult.StatusCode);
            Assert.Equal(request.EdgeType, response.EdgeType);
            Assert.Equal(request.FromNodeId, response.FromNodeId);
            Assert.Equal(request.ToNodeId, response.ToNodeId);
            Assert.Equal(nodewebPublicId, response.NodeWebId);

            //Verify
            _mockRepository.Verify(r => r.Add(It.Is<Edge>(e =>
                e.EdgeType == request.EdgeType &&
                e.FromNodeId == fromNodeUL &&
                e.ToNodeId == toNodeUL
             )), Times.Once);

            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteEdge_WhenUserIdIsNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var publicId = Guid.NewGuid();

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);

            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.DeleteEdge(publicId));
        }

        [Fact]
        public async Task DeleteEdge_WhenEdgetIsNull_ThrowsNotFoundException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetEdgeByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync((Edge)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _controller.DeleteEdge(publicId));
        }


        [Fact]
        public async Task DeleteEdge_WhenEdgeDeletionSucceeds_ReturnsNoContent()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var fromNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };
            var toNode = new Node { PublicId = Guid.NewGuid().ToMySqlBinary() };



            var existingEdge = new Edge
            {
                PublicId = publicId.ToMySqlBinary(),
                FromNode = fromNode,
                ToNode = toNode,
                FromNodeId = 10UL,
                ToNodeId = 20UL
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetEdgeByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(existingEdge);

            //Act and Assert
            var result = await _controller.DeleteEdge(publicId);
            Assert.IsType<NoContentResult>(result);

            //Verify
            _mockRepository.Verify(r => r.Delete(existingEdge), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task BulkDeleteEdges_WhenUserIdNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var userId = 1UL;
            var nodewebId = Guid.NewGuid();
            var edgeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);
            //Act and Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.BulkDeleteEdges(nodewebId, edgeIds));
        }

        [Fact]
        public async Task BulkDeleteEdges_WhenListIsEmpty_ThrowsArgumentException()
        {
            //Arrange
            var userId = 1UL;
            var nodewebId = Guid.NewGuid();
            var edgeIds = new List<Guid> { };

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)userId);
            //Act and Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _controller.BulkDeleteEdges(nodewebId, edgeIds));
        }

        [Fact]
        public async Task BulkDeleteNodes_WhenDeleteSucceeds_ReturnsNoContent()
        {
            //Arrange
            var userId = 1UL;
            var nodewebId = Guid.NewGuid();
            var edgeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)userId);
            //Act and Assert

            //return 2 which is the number of rows deleted
            _mockRepository.Setup(r => r.DeleteEdgesBulkAsync(It.IsAny<IEnumerable<byte[]>>(), It.IsAny<byte[]>(), userId))
                .ReturnsAsync(2);

            var result = await _controller.BulkDeleteEdges(nodewebId, edgeIds);

            Assert.IsType<NoContentResult>(result);

            _mockRepository.Verify(r => r.DeleteEdgesBulkAsync(
                It.Is<IEnumerable<byte[]>>(ids => ids.Count() == 2),
                It.IsAny<byte[]>(),
                userId),
                Times.Once
             );
        }
    }
}
