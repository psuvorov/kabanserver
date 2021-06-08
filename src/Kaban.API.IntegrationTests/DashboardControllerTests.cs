using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Kaban.API.Controllers;
using Kaban.API.Controllers.Requests.Boards;
using Kaban.API.Controllers.Responses.Boards;
using Xunit;

namespace Kaban.API.IntegrationTests
{
    public class DashboardControllerTests : IntegrationTest
    {
        [Fact]
        public async Task GetUserBoards_NoneOfBoardsExist_ReturnsEmptyEnumerable()
        {
            // Arrange 
            await AuthenticatedRequest();

            // Act
            var getUserBoardsResponse = await TestClient.GetAsync(ApiRoutes.Dashboard.GetUserBoards);

            // Assert
            getUserBoardsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            (await getUserBoardsResponse.Content.ReadAsAsync<IEnumerable<BoardShortInfoResponse>>()).Should().BeEmpty();
        }
        
        [Fact]
        public async Task GetUserBoards_NotAuthenticatedRequest_ReturnsUnauthorized()
        {
            // Act
            var getUserBoardsResponse = await TestClient.GetAsync(ApiRoutes.Dashboard.GetUserBoards);

            // Assert
            getUserBoardsResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        
        [Fact]
        public async Task GetUserBoards_BoardsExist_ReturnsCorrectNumberOfBoards()
        {
            // Arrange 
            await AuthenticatedRequest();
            await TestClient.PostAsJsonAsync(ApiRoutes.Dashboard.CreateBoard, new CreateBoardRequest
            {
                Name = "Test Board",
                Description = "Test Board Description"
            });

            // Act
            var getUserBoardsResponse = await TestClient.GetAsync(ApiRoutes.Dashboard.GetUserBoards);

            // Assert
            getUserBoardsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            (await getUserBoardsResponse.Content.ReadAsAsync<IEnumerable<BoardShortInfoResponse>>()).Should()
                .NotBeEmpty();
        }
        
        [Fact]
        public async Task CreateBoard_CorrectBoardData_ReturnsCreatedBoardData()
        {
            // Arrange 
            await AuthenticatedRequest();
            
            // Act
            await TestClient.PostAsJsonAsync(ApiRoutes.Dashboard.CreateBoard, new CreateBoardRequest
            {
                Name = "Test Board",
                Description = "Test Board Description"
            });

            // Assert
            var getUserBoardsResponse = await TestClient.GetAsync(ApiRoutes.Dashboard.GetUserBoards);
            getUserBoardsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var boardShortInfoResponses = (await getUserBoardsResponse.Content.ReadAsAsync<IEnumerable<BoardShortInfoResponse>>()).ToList();
            boardShortInfoResponses.Should().NotBeEmpty();
            boardShortInfoResponses.FirstOrDefault().Should().NotBeNull();
            boardShortInfoResponses.FirstOrDefault()?.Name.Should().Be("Test Board");
        }
        
        [Fact]
        public async Task CreateBoard_EmptyBoardData_ReturnsBadRequest()
        {
            // Arrange 
            await AuthenticatedRequest();
            
            // Act
            var createBoardResponse = await TestClient.PostAsJsonAsync(ApiRoutes.Dashboard.CreateBoard, new CreateBoardRequest());

            // Assert
            createBoardResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task CreateBoard_AttemptToCreateDuplicate_ReturnsBadRequest()
        {
            // Arrange 
            await AuthenticatedRequest();
            var createBoardRequest = new CreateBoardRequest
            {
                Name = "Test Board",
                Description = "Test Board Description"
            };

            // Act
            await TestClient.PostAsJsonAsync(ApiRoutes.Dashboard.CreateBoard, createBoardRequest);
            var createBoardResponse = await TestClient.PostAsJsonAsync(ApiRoutes.Dashboard.CreateBoard, createBoardRequest);

            // Assert
            createBoardResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}