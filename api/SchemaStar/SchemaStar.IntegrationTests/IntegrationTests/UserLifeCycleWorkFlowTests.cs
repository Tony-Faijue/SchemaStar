using Microsoft.AspNetCore.Mvc.Testing;
using SchemaStar.DTOs;
using SchemaStar.DTOs.Authentication_DTOs;
using SchemaStar.IntegrationTests.Factory;
using System.Net;

namespace SchemaStar.IntegrationTests.IntegrationTests
{
    /// <summary>
    /// Test the common work flows of the users on the User object 
    /// </summary>
    public class UserLifeCycleWorkFlowTests : IClassFixture<CustomizeWebApplicationFactory<Program>>
    {
        private readonly CustomizeWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public UserLifeCycleWorkFlowTests(CustomizeWebApplicationFactory<Program> factory, ITestOutputHelper output) 
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });
            _output = output;
        }

        [Fact]
        public async Task FullUserLifeCycle_RegisterLoginUpdateDelete_WorkflowSucceeds() 
        {
            //Register -> Login -> Read self -> Update -> Delete -> Confirm Deletion
            //Arrange Act Assert in each section

            string email = "lifecycle@example.com";
            string password = "StrongPass99!";
            string userName = "lifecycle_user";

            //---Register---

            var registerRequest = new RegisterUserRequestDTO { Username = userName, Email = email, Password = password };

            var registerResponse = await _client.PostAsJsonAsync("/api/Users", registerRequest);

            Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
            var registered = await registerResponse.Content.ReadFromJsonAsync<UserResponseDTO>();
            Assert.NotNull(registered);
            Assert.Equal(email, registered.Email);

            //Public Id of the User
            var publicId = registered.PublicId;
            Assert.NotEqual(Guid.Empty, publicId);

            //---Login---
            
            var loginRequest = new TokenRequestModel { Email = email, Password = password };

            var loginResponse = await _client.PostAsJsonAsync("/api/Users/token", loginRequest);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var loginBody = await loginResponse.Content.ReadFromJsonAsync<CookieAuthResponseDTO>();
            Assert.NotNull(loginBody);
            Assert.True(loginBody.IsAuthenticated);

            //---Read Self(Current user at protected endpoint at '/me') ---

            var meResponse = await _client.GetAsync("/api/Users/me");
            Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);

            var currentUser = await meResponse.Content.ReadFromJsonAsync<UserResponseDTO>();
            Assert.NotNull(currentUser);
            Assert.Equal(email, currentUser.Email);
            Assert.Equal(publicId, currentUser.PublicId);

            //---Update---
            var updateRequest = new UpdateUserRequestDTO { Username = "updated_lifecycle_user" };

            var updatedResponse = await _client.PatchAsJsonAsync($"/api/Users/{publicId}", updateRequest);

            Assert.Equal(HttpStatusCode.OK, updatedResponse.StatusCode);
            var updatedBody = await updatedResponse.Content.ReadFromJsonAsync<UserResponseDTO>();
            Assert.NotNull(updatedBody);
            Assert.Equal("updated_lifecycle_user", updatedBody.Username);

            //---Delete---
            var deleteResponse = await _client.DeleteAsync($"/api/Users/{publicId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            //---Confirm Deletion---
            var confirmDeletionResponse = await _client.GetAsync($"/api/Users/{publicId}");
            Assert.Equal(HttpStatusCode.NotFound, confirmDeletionResponse.StatusCode);
        }

        [Fact]
        public async Task CrossUserAccess_UpdateAndDelete_ReturnsForbiddenException() 
        {
            //Register User A -> Register User B -> Login as User B -> Try Updating User A (Reject)-> Try Deleting User A (Reject)

            //Register User A

            var registerRequestUser_A = new RegisterUserRequestDTO { Username = "user_a", Email = "user_a@example.com", Password = "StrongPassword99!" };
            var registerResponseUser_A = await _client.PostAsJsonAsync("/api/Users", registerRequestUser_A);
            Assert.Equal(HttpStatusCode.Created, registerResponseUser_A.StatusCode);

            var userA = await registerResponseUser_A.Content.ReadFromJsonAsync<UserResponseDTO>();
            Assert.NotNull(userA);
            var userAPublicId = userA.PublicId;

            //Register User B

            var registerRequestUser_B = new RegisterUserRequestDTO { Username = "user_b", Email = "user_b@example.com", Password = "Password1!" };
            var registerResponseUser_B = await _client.PostAsJsonAsync("/api/Users", registerRequestUser_B);
            Assert.Equal(HttpStatusCode.Created, registerResponseUser_B.StatusCode);

            //Login as User B

            var loginUser_B = new TokenRequestModel { Email = "user_b@example.com", Password = "Password1!" };
            var loginResponse = await _client.PostAsJsonAsync("/api/Users/token", loginUser_B);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            //Try Updating User A as User B

            var updateUser_A = new UpdateUserRequestDTO { Username = "hacked_username" };
            var updatedResponseUser_A = await _client.PatchAsJsonAsync($"/api/Users/{userAPublicId}", updateUser_A);
            Assert.Equal(HttpStatusCode.Forbidden, updatedResponseUser_A.StatusCode);

            //Try Deleting User A as User B
            var deleteUser_A = await _client.DeleteAsync($"/api/Users/{userAPublicId}");
            Assert.Equal(HttpStatusCode.Forbidden, deleteUser_A.StatusCode);
        }
    }
}
