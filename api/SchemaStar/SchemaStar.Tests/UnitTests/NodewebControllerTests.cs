using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using NuGet.ContentModel;
using SchemaStar.Controllers;
using SchemaStar.DataRepositories;
using SchemaStar.DTOs.Nodeweb_DTOs;
using SchemaStar.Exceptions;
using SchemaStar.Models;
using SchemaStar.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchemaStar.Tests.UnitTests
{
    public class NodewebControllerTests
    {
        private readonly NodewebsController _controller;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<INodewebRepository> _mockRepository;
        private readonly Mock<ILogger<NodewebsController>> _mockLogger;

        public NodewebControllerTests()
        {
            //Mock services
            _mockRepository = new Mock<INodewebRepository>();
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<NodewebsController>>();

            //Initialize controller
            _controller = new NodewebsController(
                _mockUserService.Object,
                _mockLogger.Object,
                _mockRepository.Object
            );
        }

        [Fact]
        public async Task GetNodewebs_WhenUserIdNull_ThrowsUnauthorizedException()
        {
            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);
            //Act and Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.GetNodewebs());
        }

        [Fact]
        public async Task GetNodewebs_WhenUserIdIsValid_ReturnsListOfNodewebResponseDTOs()
        {
            //Arrange
            var userId = 1UL;
            var listOfNodewebs = new List<Nodeweb> {
                new Nodeweb { PublicId = Guid.NewGuid().ToMySqlBinary(), NodeWebName = "Web 1" },
                new Nodeweb { PublicId = Guid.NewGuid().ToMySqlBinary(), NodeWebName = "Web 2" }
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetAllNodewebsByUserIdAsync(userId)).ReturnsAsync(listOfNodewebs);

            //Act and Assert
            var result = await _controller.GetNodewebs();

            var actionResult = Assert.IsType<ActionResult<IEnumerable<NodewebResponseDTO>>>(result);
            Assert.NotNull(actionResult.Value);
            Assert.Equal(2, actionResult.Value.Count());
        }

        [Fact]
        public async Task GetNodeweb_WhenUserIdNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var publicId = Guid.NewGuid();
            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);
            //Act and Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.GetNodeweb(publicId));
        }

        [Fact]
        public async Task GetNodeweb_WhenNodewebDoesNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodewebByPublicIdAsync(It.IsAny<byte[]>(), userId))
                .ReturnsAsync((Nodeweb)null);

            //Act and Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetNodeweb(publicId));
        }


        [Fact]
        public async Task GetNodeweb_WhenGettingValidNodeweb_ReturnsNodewebResponseDTO()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var existingNodeweb = new Nodeweb
            {
                PublicId = publicId.ToMySqlBinary(),
                NodeWebName = "Test_Nodeweb",
                CreatedAt = now,
                UpdatedAt = now,
                LastLayoutAt = now
            };

            //Mock the needed services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodewebByPublicIdAsync(It.IsAny<byte[]>(), userId))
                .ReturnsAsync(existingNodeweb);

            //Act and Assert
            var result = await _controller.GetNodeweb(publicId);

            var response = Assert.IsType<NodewebResponseDTO>(result.Value);

            Assert.Equal(publicId, response.PublicId);
            Assert.Equal("Test_Nodeweb", response.NodeWebName);
            Assert.Equal(now, response.CreatedAt);
        }

        [Fact]
        public async Task UpdateNodeweb_WhenUserIdIsNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var publicId = Guid.NewGuid();
            var request = new NodewebRequestDTO {
                NodeWebName = "Updated Nodeweb"
            };
            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);

            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.UpdateNodeweb(publicId, request));
        }

        [Fact]
        public async Task UpdateNodeweb_WhenNodewebDoesNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var request = new NodewebRequestDTO
            {
                NodeWebName = "Updated Nodeweb"
            };
            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodewebByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync((Nodeweb)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _controller.UpdateNodeweb(publicId, request));
        }

        [Fact]
        public async Task UpdateNodeweb_WhenDuplicateNodewebNameFound_ThrowsConflictException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var existingNodeweb = new Nodeweb
            {
                PublicId = publicId.ToMySqlBinary(),
                NodeWebName = "Test_Nodeweb",
                CreatedAt = now,
                UpdatedAt = now,
                LastLayoutAt = now
            };

            var request = new NodewebRequestDTO
            {
                NodeWebName = "Updated Nodeweb"
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodewebByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(existingNodeweb);

            _mockRepository.Setup(r => r.ExistsByNameAsync(userId, request.NodeWebName, It.IsAny<byte[]>())).ReturnsAsync(true);
                
            await Assert.ThrowsAsync<ConflictException>(() => _controller.UpdateNodeweb(publicId, request));
        }

        [Fact]
        public async Task UpdateNodeweb_WhenNodewebUpdatedConcurrently_ThrowsConflictException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var existingNodeweb = new Nodeweb
            {
                PublicId = publicId.ToMySqlBinary(),
                NodeWebName = "Test_Nodeweb",
                CreatedAt = now,
                UpdatedAt = now,
                LastLayoutAt = now
            };

            var request = new NodewebRequestDTO
            {
                NodeWebName = "Updated Nodeweb"
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodewebByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(existingNodeweb);
            //Force a DbUpdateException
            _mockRepository.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new DbUpdateConcurrencyException());

            await Assert.ThrowsAsync<ConflictException>(() => _controller.UpdateNodeweb(publicId, request)); //Should throw conflict excpetion
        }

        [Fact]
        public async Task UpdateNodeweb_WhenNodewebNameIsInvalid_ThrowsArgumentException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var existingNodeweb = new Nodeweb
            {
                PublicId = publicId.ToMySqlBinary(),
                NodeWebName = "Test_Nodeweb",
                CreatedAt = now,
                UpdatedAt = now,
                LastLayoutAt = now
            };

            var request = new NodewebRequestDTO
            {
                NodeWebName = "Updated Nodeweb"
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodewebByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(existingNodeweb);
            //Force a DbUpdateException
            _mockRepository.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new ArgumentException());

            await Assert.ThrowsAsync<ArgumentException>(() => _controller.UpdateNodeweb(publicId, request));
        }

        [Fact]
        public async Task UpdateNodeweb_WhenNodewebUpdateSucceeds_ReturnsNoContent()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var existingNodeweb = new Nodeweb
            {
                PublicId = publicId.ToMySqlBinary(),
                NodeWebName = "Test_Nodeweb",
                CreatedAt = now,
                UpdatedAt = now,
                LastLayoutAt = now
            };

            var request = new NodewebRequestDTO
            {
                NodeWebName = "Updated Nodeweb"
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodewebByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(existingNodeweb);

            _mockRepository.Setup(r => r.ExistsByNameAsync(userId, request.NodeWebName, It.IsAny<byte[]>())).ReturnsAsync(false);

            //Act and assert
            var result = await _controller.UpdateNodeweb(publicId, request);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal("Updated Nodeweb", existingNodeweb.NodeWebName);

            //Verify
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task PostNodeweb_WhenUserIdIsNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var userId = 1UL;
            var request = new NodewebRequestDTO
            {
                NodeWebName = "New Nodeweb"
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);

            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.PostNodeweb(request));
        }

        [Fact]
        public async Task PostNodeweb_WhenDuplicateNodewebNameFound_ThrowsConflictException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var request = new NodewebRequestDTO
            {
                NodeWebName = "New Nodeweb"
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.ExistsByNameAsync(userId, request.NodeWebName, null)).ReturnsAsync(true);

            await Assert.ThrowsAsync<ConflictException>(() => _controller.PostNodeweb(request));
        }

        [Fact]
        public async Task PostNodeweb_WhenNodewebCreationsSucceeds_Returns201CreatedActionResult()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var request = new NodewebRequestDTO
            {
                NodeWebName = "New Nodeweb"
            };

            var newNodeweb = new Nodeweb
            {
                PublicId = publicId.ToMySqlBinary(),
                NodeWebName = request.NodeWebName,
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.ExistsByNameAsync(userId, request.NodeWebName, null)).ReturnsAsync(false);

            //Act and Assert
            var result = await _controller.PostNodeweb(request);
            
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var response = Assert.IsType<NodewebResponseDTO>(createdAtResult.Value);

            Assert.Equal(201, createdAtResult.StatusCode);
            Assert.Equal(request.NodeWebName, response.NodeWebName);
            Assert.NotEqual(Guid.Empty, response.PublicId);

            //Verify
            _mockRepository.Verify(r => r.Add(It.IsAny<Nodeweb>()), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteNodeweb_WhenUserIdIsNull_ThrowsUnauthorizedException()
        {
            //Arrange
            var publicId = Guid.NewGuid();

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns((ulong?)null);

            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.DeleteNodeweb(publicId));
        }

        [Fact]
        public async Task DeleteNodeweb_WhenNodewebIsNull_ThrowsNotFoundException()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodewebByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync((Nodeweb)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _controller.DeleteNodeweb(publicId));
        }

        [Fact]
        public async Task DeleteNodeweb_WhenNodewebDeletionSucceeds_ReturnsNoContent()
        {
            //Arrange
            var userId = 1UL;
            var publicId = Guid.NewGuid();

            var existingNodeweb = new Nodeweb
            {
                PublicId = publicId.ToMySqlBinary(),
                NodeWebName = "Test_Nodeweb",
            };

            //Mock services
            _mockUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

            _mockRepository.Setup(r => r.GetNodewebByPublicIdAsync(It.IsAny<byte[]>(), userId)).ReturnsAsync(existingNodeweb);

            //Act and Assert
            var result = await _controller.DeleteNodeweb(publicId);
            Assert.IsType<NoContentResult>(result);
            
            //Verify
            _mockRepository.Verify(r => r.Delete(existingNodeweb), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
