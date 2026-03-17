using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using SchemaStar.DTOs;
using SchemaStar.Exceptions;
using SchemaStar.JWT;
using SchemaStar.Models;
using SchemaStar.Services;
using System.Threading.Tasks;

namespace SchemaStar.Tests.UnitTests;

public class UserServiceTests
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly UserService _userService;

    /// <summary>
    /// Initialize and mock the depedencies needed to test the UserService
    /// </summary>
    public UserServiceTests() 
    {
        //Setup to mock UserManager
        var store = new Mock<IUserStore<User>>();
        _mockUserManager = new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        //Mock other dependencies as null/base objects
        var mockSignInManager = new Mock<SignInManager<User>>(_mockUserManager.Object, new Mock<IHttpContextAccessor>().Object, new Mock<IUserClaimsPrincipalFactory<User>>().Object, null!, null!, null!, null!);
        var mockJwtOptions = new Mock<IOptions<JWTOptions>>();
        mockJwtOptions.Setup(x => x.Value).Returns(new JWTOptions());
        var mockHttpContext = new Mock<IHttpContextAccessor>();
        var mockWebEnv = new Mock<IWebHostEnvironment>();

        //Initialize user service with mocks of other dependencies
        _userService = new UserService(
            _mockUserManager.Object,
            mockSignInManager.Object,
            mockJwtOptions.Object,
            mockHttpContext.Object,
            mockWebEnv.Object
         );
    }

    [Fact]
    public async Task RegisterUserAysnc_WhenCreatingNewUserWithExistingEmail_ThrowsConflictException()
    {
     //--Arrange--
        var request = new RegisterUserRequestDTO
        {
            Email = "testUser@example.com",
            Username = "NewUser",
            Password = "Password1!"
        };

        //Create a dummy existing user
        var existingUser = new User { Email = request.Email };

        //Mock the FindByEmail action
        _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(existingUser);

        //--Act and Assert--
        await Assert.ThrowsAsync<ConflictException>(() => _userService.RegisterUserAsync(request));

        //Additional verification to ensure the code stopped early not trying to findByName or create user
        _mockUserManager.Verify(x => x.FindByNameAsync(It.IsAny<string>()), Times.Never);
        _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }
    
    [Fact]
    public async Task RegisterUserAysnc_WhenCreatingNewUserWithExistingUsername_ThrowsConflictException() 
    {
        //--Arrange--
        var request = new RegisterUserRequestDTO
        {
            Email = "testUser@example.com",
            Username = "NewUser",
            Password = "Password1!"
        };

        //Create a dummy existing user
        var existingUser = new User { UserName = request.Username };

        //Email check happens before username check and returns null to continue
        _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync((User?)null);

        //Mock the FindByName action
        _mockUserManager.Setup(x => x.FindByNameAsync(request.Username)).ReturnsAsync(existingUser);

        //--Act and Assert--
        await Assert.ThrowsAsync<ConflictException>(() => _userService.RegisterUserAsync(request));

        //Additional verification to ensure the code checks emails once and does not create a user
        _mockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUserAysnc_WhenCreatingNewUserFails_ThrowsInvalidOperationException() 
    {
        //--Arrange--
        var request = new RegisterUserRequestDTO
        {
            Email = "testUser@example.com",
            Username = "NewUser",
            Password = "Password1!"
        };

        //Email check
        _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync((User?)null);

        //Username check
        _mockUserManager.Setup(x => x.FindByNameAsync(request.Username)).ReturnsAsync((User?)null);

        //Setup for failed IdentityResult errors
        var identityErrors = new[] {
            new IdentityError { Description = "Password is too weak." },
            new IdentityError { Description = "Username is invalid." },
            new IdentityError { Description = "Email is invalid." },
        };

        var failedResult = IdentityResult.Failed(identityErrors);

        //Failed creating the user
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password)).ReturnsAsync(failedResult);

        //--Act and Assert--
       var exception =  await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.RegisterUserAsync(request));

        //Assert error messages were formatted correctly
        Assert.Contains("Password is too weak.", exception.Message);
        Assert.Contains("Username is invalid.", exception.Message);
        Assert.Contains("Email is invalid.", exception.Message);

        //Verifications
        _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
        _mockUserManager.Verify(x => x.FindByNameAsync(request.Username), Times.Once);

        //Verify that create method has been called
        _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<User>(), request.Password), Times.Once);
    }

    [Fact]
    public async Task RegisterUserAysnc_WhenCreatingNewUserSucceeds_ReturnUserResponseDTO() 
    {
        //--Arrange--
        var request = new RegisterUserRequestDTO
        {
            Email = "testUser@example.com",
            Username = "NewUser",
            Password = "Password1!"
        };

        //Email check
        _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync((User?)null);
        //Username check
        _mockUserManager.Setup(x => x.FindByNameAsync(request.Username)).ReturnsAsync((User?)null);

        //Succesfully created the user
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password)).ReturnsAsync(IdentityResult.Success);

        //--Act and Assert--
        var result = await _userService.RegisterUserAsync(request);

        Assert.NotNull(result);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.Username, result.Username);

        Assert.NotEqual(Guid.Empty, result.PublicId);

        
        //Verifications
        _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
        _mockUserManager.Verify(x => x.FindByNameAsync(request.Username), Times.Once);

        //Verify that create method has been called
        _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<User>(), request.Password), Times.Once);
    }

}
