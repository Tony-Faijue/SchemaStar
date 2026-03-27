using Castle.Core.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SchemaStar.DTOs;
using SchemaStar.DTOs.Authentication_DTOs;
using SchemaStar.Exceptions;
using SchemaStar.JWT;
using SchemaStar.Models;
using SchemaStar.Services;
using System.Threading.Tasks;

namespace SchemaStar.Tests.UnitTests;

public class UserServiceTests
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<SignInManager<User>> _mockSignInManager;
    private readonly Mock<IOptions<JWTOptions>> _mockJwtOptions;
    private readonly Mock<IHttpContextAccessor> _mockHttpContext;
    private readonly UserService _userService;

    /// <summary>
    /// Initialize and mock the depedencies needed to test the UserService
    /// </summary>
    public UserServiceTests() 
    {
        //Setup to mock UserManager
        var store = new Mock<IUserStore<User>>();
        _mockUserManager = new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        //Initialize SignIn Managaer
        _mockSignInManager = new Mock<SignInManager<User>>(
            _mockUserManager.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<User>>().Object,
            null!, null!, null!, null!
         );

        //Initialize JwtOptions
        _mockJwtOptions = new Mock<IOptions<JWTOptions>>();
        //default value
        _mockJwtOptions.Setup(x => x.Value).Returns(new JWTOptions
        {
            Key = "SuperSecretTestingKey1234567890123456",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            DurationInMinutes = 60
        });

        //Mock other dependencies as null/base objects
        _mockHttpContext = new Mock<IHttpContextAccessor>();
        var mockWebEnv = new Mock<IWebHostEnvironment>();
        var mockLogger = new Mock<ILogger<UserService>>();

        //Initialize user service with mocks of other dependencies
        _userService = new UserService(
            _mockUserManager.Object,
            _mockSignInManager.Object,
            _mockJwtOptions.Object,
            _mockHttpContext.Object,
            mockWebEnv.Object,
            mockLogger.Object
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

        //Mock the FindByEmail and FindByName actions 
        _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(existingUser);
        _mockUserManager.Setup(x => x.FindByNameAsync(request.Username)).ReturnsAsync((User?)null);
        //--Act and Assert--
        await Assert.ThrowsAsync<ConflictException>(() => _userService.RegisterUserAsync(request));

        //Additional verification to ensure FindByEmail and FindByName called once or create user
        _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
        _mockUserManager.Verify(x => x.FindByNameAsync(request.Username), Times.Once);
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

        //Mock both FindBy async methods
        _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync((User?)null);
        _mockUserManager.Setup(x => x.FindByNameAsync(request.Username)).ReturnsAsync(existingUser);

        //--Act and Assert--
        await Assert.ThrowsAsync<ConflictException>(() => _userService.RegisterUserAsync(request));

        //Additional verification to ensure the code checks both methods called onceand does not create a user
        _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
        _mockUserManager.Verify(x => x.FindByNameAsync(request.Username), Times.Once);

        _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUserAsync_WhenCreateAsyncReturnsDuplicateError_ThrowsConflictException() 
    {
        //--Arrange--
        var request = new RegisterUserRequestDTO
        {
            Email = "testUser@example.com",
            Username = "NewUser",
            Password = "Password1!"
        };
        //Mock both methods
        _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync((User?)null);
        _mockUserManager.Setup(x => x.FindByNameAsync(request.Username)).ReturnsAsync((User?)null);

        var duplicateErrors = new[]
        {
            new IdentityError { Code = "DuplicateEmail", Description = "Email already taken."}
        };

        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password)).ReturnsAsync(IdentityResult.Failed(duplicateErrors));

        //--Act and Assert--
        await Assert.ThrowsAsync<ConflictException>(() => _userService.RegisterUserAsync(request));

        _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<User>(), request.Password), Times.Once);
    }

    [Fact]
    public async Task RegisterUserAysnc_WhenCreatingNewUserFails_ThrowsValidationException() 
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
       var exception =  await Assert.ThrowsAsync<ValidationException>(() => _userService.RegisterUserAsync(request));

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

    [Fact]
    public async Task GetTokenWithCookieAsync_WhenUserIsNull_ThrowsUnauthorizedException()
    {
        //--Arrange--
        var model = new TokenRequestModel { Email = "testUser@example.com" };

        _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync((User?)null);

        //Act and Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _userService.GetTokenWithCookieAsync(model));

        //Verify
        _mockUserManager.Verify(x => x.FindByEmailAsync(model.Email), Times.Once);
    }

    [Fact]
    public async Task GetTokenWithCookieAsync_WhenPasswordCheckFails_ThrowsUnauthorizedException()
    {
        //--Arrange--
        var model = new TokenRequestModel { Email = "testUser@example.com", Password = "WrongPassword!" };
        var existingUser = new User { Email = model.Email};

        //Mock Email Check Succeeds
        _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(existingUser);
        
        //Mock Password Check Fails
        _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(
            existingUser,
            model.Password,
            false)
        ).ReturnsAsync(SignInResult.Failed);
        
        //Act and Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _userService.GetTokenWithCookieAsync(model));

        //Verify
        _mockUserManager.Verify(x => x.FindByEmailAsync(model.Email), Times.Once);
        _mockSignInManager.Verify(x => x.CheckPasswordSignInAsync(existingUser, model.Password, false), Times.Once);
    }

    //Example of testing with mutliple parameters; testing with one value invalid is fine since it translates to the other scenarios
    [Theory]
    [InlineData(null, "Issuer", "Audience", 60)]
    [InlineData("SecretKey", "", "Audience", 60)]
    [InlineData("SecretKey", "Issuer", null, 60)]
    [InlineData("SecretKey", "Issuer", "Audience", 0)]

    public async Task GetTokenWithCookieAsync_WhenJwtConfigIsInvalid_ThrowsInvalidOperationException(
        string? key, string? issuer, string? audience, int duration)
    {
        //--Arrange--
        var model = new TokenRequestModel { Email = "testUser@example.com", Password = "WrongPassword!" };
        var existingUser = new User { Email = model.Email, UserName = "testuser", PublicId = new byte[16] };

        //Mock Email Check Succeeds
        _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(existingUser);

        //Mock Password Check Succeeds
        _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(
            existingUser,
            model.Password,
            false)
        ).ReturnsAsync(SignInResult.Success);

        //MockJwtOptions
        _mockJwtOptions.Setup(x => x.Value).Returns(new JWTOptions
        {
            Key = key,
            Issuer = issuer,
            Audience = audience,
            DurationInMinutes = duration
        });

        //Act and Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.GetTokenWithCookieAsync(model));

        //Verify
        _mockUserManager.Verify(x => x.FindByEmailAsync(model.Email), Times.Once);
        _mockSignInManager.Verify(x => x.CheckPasswordSignInAsync(existingUser, model.Password, false), Times.Once);
    }

    [Fact]
    public async Task GetTokenWithCookieAsync_WhenCookieGenerationANDCookieAppendingSucceeds_ReturnsCookieAuthResponseDTO()
    {
        //--Arrange--
        var model = new TokenRequestModel { Email = "testUser@example.com", Password = "WrongPassword!" };
        var existingUser = new User 
        { 
            Email = model.Email,
            UserName = "SuccessUser",
            PublicId = Guid.NewGuid().ToByteArray(),//can't be null
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        //Mock Email Check Succeeds
        _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(existingUser);

        //Mock Password Check Succeeds
        _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(
            existingUser,
            model.Password,
            false)
        ).ReturnsAsync(SignInResult.Success);


        //Mock the Cookie
        var mockResponse = new Mock<HttpResponse>();
        var mockCookies = new Mock<IResponseCookies>();
        var mockContext = new Mock<HttpContext>();

        //Link Context -> Response -> Cookies
        mockContext.Setup(c => c.Response).Returns(mockResponse.Object);
        mockResponse.Setup(r => r.Cookies).Returns(mockCookies.Object);
        _mockHttpContext.Setup(x => x.HttpContext).Returns(mockContext.Object);

        //--Act--
        var result =  await _userService.GetTokenWithCookieAsync(model);

        //--Assert--
        Assert.True(result.IsAuthenticated);
        Assert.Equal(existingUser.Email, result.Email);

        //Verify
        _mockUserManager.Verify(x => x.FindByEmailAsync(model.Email), Times.Once);
        _mockSignInManager.Verify(x => x.CheckPasswordSignInAsync(existingUser, model.Password, false), Times.Once);

        //Verify the cookie was appended
        mockCookies.Verify(x => x.Append(
            "authToken",
            It.IsAny<string>(),
            It.IsAny<CookieOptions>()),
            Times.Once);
    }
}
