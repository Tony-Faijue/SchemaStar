using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
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
        public async Task GetNodeweb_ReturnsNotFound_WhenNodewebDoesNotExist()
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

    }
}
