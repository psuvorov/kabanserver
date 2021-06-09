using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Kaban.API.Controllers.Requests.Users;
using Kaban.API.Controllers.Responses;
using Kaban.API.Controllers.Responses.Users;
using Xunit;

namespace Kaban.API.IntegrationTests
{
    public class UsersControllerTests : IntegrationTest
    {
        [Fact]
        public async Task RegisterUser_CorrectCredentials_ReturnsJustCreatedUserId()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                FirstName = "user1",
                LastName = "user1",
                Username = "user1",
                Email = "user1@mail.com",
                Password = "UsEr1"
            };

            // Act
            var registerUser = await RegisterUserAsync(request);
            
            // Assert
            registerUser.StatusCode.Should().Be(HttpStatusCode.OK);
            (await registerUser.Content.ReadAsAsync<EntityCreatingSuccessResponse>()).EntityId.Should().NotBeEmpty();
        }
        
        [Fact]
        public async Task RegisterUser_EmptyCredentials_ReturnsBadRequest()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                FirstName = "",
                LastName = "",
                Username = "",
                Email = "",
                Password = ""
            };

            // Act
            var registerUser = await RegisterUserAsync(request);
            
            // Assert
            registerUser.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task RegisterUser_SameCredentials_ReturnsBadRequest()
        {
            // Arrange
            var user = new RegisterUserRequest
            {
                FirstName = "user1",
                LastName = "user1",
                Username = "user1",
                Email = "user1@mail.com",
                Password = "UsEr1"
            };

            // Act
            var registerUser1 = await RegisterUserAsync(user);
            var registerUser2 = await RegisterUserAsync(user);
            
            // Assert
            registerUser1.StatusCode.Should().Be(HttpStatusCode.OK);
            (await registerUser1.Content.ReadAsAsync<EntityCreatingSuccessResponse>()).EntityId.Should().NotBeEmpty();
            registerUser2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            (await registerUser2.Content.ReadAsAsync<OperationFailureResponse>()).Message.Should().NotBeEmpty();
        }
        
        [Fact]
        public async Task AuthenticateUser_CorrectCredentials_ReturnsJwt()
        {
            // Arrange
            var registerUser = new RegisterUserRequest
            {
                FirstName = "user1",
                LastName = "user1",
                Username = "user1",
                Email = "user1@mail.com",
                Password = "UsEr1"
            };
            await RegisterUserAsync(registerUser);
            var request = new AuthenticateRequest
            {
                Email = registerUser.Email,
                Password = registerUser.Password
            };

            // Act
            var authenticateResponse = await AuthenticateAsync(request);

            // Assert
            authenticateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            (await authenticateResponse.Content.ReadAsAsync<AuthSuccessResponse>()).Token.Should().NotBeEmpty();
        }
        
        [Fact]
        public async Task AuthenticateUser_IncorrectCredentials_ReturnsBadRequest()
        {
            // Arrange
            var request = new AuthenticateRequest
            {
                Email = "dummy email",
                Password = "dummy password"
            };

            // Act
            var authenticateResponse = await AuthenticateAsync(request);

            // Assert
            authenticateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            (await authenticateResponse.Content.ReadAsAsync<OperationFailureResponse>()).Message.Should().NotBeEmpty();
        }
        
    }
}