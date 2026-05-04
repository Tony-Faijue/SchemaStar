using Microsoft.AspNetCore.Mvc.Testing;
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

        private const string SeedEmail = "test@example.com";
        private const string SeedPassword = "Password123!";
        public AuthenticationFlowTests(CustomizeWebApplicationFactory<Program> factory) 
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true});   //set UseCookies = true so the HttpClient stores and resend Set-Cookie headers
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
        public async Task After_Login_CookieGrantsAccessToProtectedEndpoint()
        {

        }

        [Fact]
        public async Task AccessProtectedEndpoint_Without_Cookie_Returns_UnauthorizedException()
        {

        }

        [Fact]
        public async Task AccessProtectedEndpoint_With_InvalidCookie_Returns_UnauthorizedException()
        {

        }
    }
}
