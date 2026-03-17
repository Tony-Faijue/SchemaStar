using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using SchemaStar.JWT;
using SchemaStar.Models;
using SchemaStar.Services;

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
    public void RegisterUserAysnc_WhenCreatingNewUserWithExistingEmail_ThrowsConflictException()
    {
        //Arrange
        //Act
        //Assert
    }
}
