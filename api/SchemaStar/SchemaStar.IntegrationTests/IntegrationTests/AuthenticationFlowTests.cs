using Microsoft.AspNetCore.Mvc.Testing;
using NuGet.Protocol.Plugins;
using SchemaStar.DTOs;
using SchemaStar.DTOs.Authentication_DTOs;
using SchemaStar.IntegrationTests.Factory;
using System.Net;

namespace SchemaStar.IntegrationTests.IntegrationTests
{
    /// <summary>
    /// Tests the (BFF) Authentication Flow
    /// 
    /// </summary>
    public class AuthenticationFlowTests : IClassFixture<CustomizeWebApplicationFactory<Program>>
    {
        private readonly CustomizeWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        private const string SeedEmail = "test@example.com";
        private const string SeedPassword = "Password123!";
        public AuthenticationFlowTests(CustomizeWebApplicationFactory<Program> factory, ITestOutputHelper output) 
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true});   //set UseCookies = true so the HttpClient stores and resend Set-Cookie headers
            _output = output;
        }

        [Fact]
        public async Task Login_With_ValidCredentials_Succeeds_And_SetsHttpOnlyCookie()
        {
            //Arrange
            var loginRequest = new TokenRequestModel
            {
                Email = SeedEmail,
                Password = SeedPassword,
            };

            //Act 
            var response = await _client.PostAsJsonAsync("/api/Users/token", loginRequest);

            // Log unexepected failure
            if (response.StatusCode != HttpStatusCode.OK)
                _output.WriteLine($"Unexpected login failure: {await response.Content.ReadAsStringAsync()}");

            //Assert

            //HTTP Status
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            //Response Body
            var body = await response.Content.ReadFromJsonAsync<CookieAuthResponseDTO>();

            Assert.NotNull(body);
            Assert.True(body.IsAuthenticated);
            Assert.Equal(SeedEmail, body.Email);
            
            //Set-Cookie Header Exists
            var setCookieHeaders = response.Headers
                .Where(h => h.Key.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase))
                .SelectMany(h => h.Value)
                .ToList();
            Assert.NotEmpty(setCookieHeaders);
            //authCookie exists
            var authCookie = setCookieHeaders.FirstOrDefault(c =>
                c.StartsWith("authToken", StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(authCookie);

            //HttpOnly exists in authCookie
            Assert.Contains("HttpOnly", authCookie, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task After_Login_CookieGrantsAccessToProtectedEndpoint_GetCurrentUserInfo()
        {
            //Arrange

            //First Login with Cookie and Confirm it succeeds (Login Process)
            var loginRequest = new TokenRequestModel
            {
                Email = SeedEmail,
                Password = SeedPassword,
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/Users/token", loginRequest);

            // Log unexpected failure
            if (loginResponse.StatusCode != HttpStatusCode.OK)
                _output.WriteLine($"Unexpected login failure: {await loginResponse.Content.ReadAsStringAsync()}");

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            //Act

            //Access the me endpoint with the stored cookie
            var meResponse = await _client.GetAsync("/api/Users/me");

            // Log unexpected failure
            if (meResponse.StatusCode != HttpStatusCode.OK)
                _output.WriteLine($"Unexpected login failure: {await meResponse.Content.ReadAsStringAsync()}");

            //Assert
            Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);

            var userInfo = await meResponse.Content.ReadFromJsonAsync<UserResponseDTO>();

            Assert.NotNull(userInfo);
            Assert.True(userInfo.IsAuthenticated);
            Assert.Equal(SeedEmail, userInfo.Email);

        }

        [Fact]
        public async Task AccessProtectedEndpoint_Without_Cookie_Returns_UnauthorizedException()
        {
            //Arrange
            //No login to have empty cookie

            //Act
            var response = await _client.GetAsync("/api/Users/me");

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        }

        [Fact]
        public async Task AccessProtectedEndpoint_With_InvalidCookie_Returns_UnauthorizedException()
        {
            //Arrange
            //Inject invalid/broken JWT token as authToken Cookie
            const string brokenToken = "authToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9" +
                ".TAMPERED_PAYLOAD" + 
                ".INVALID_SIGNATURE";

            _client.DefaultRequestHeaders.Add("Cookie", brokenToken);

            //Act
            var response = await _client.GetAsync("/api/Users/me");

            //Log unexpected failure
            if (response.StatusCode != HttpStatusCode.Unauthorized)
                _output.WriteLine($"Unexpected login failure: {await response.Content.ReadAsStringAsync()}");

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        }
    }
}
