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
            var getUserBoardsResponse = await TestClient.GetAsync(ApiRoutes.Dashboards.GetUserBoards);

            // Assert
            getUserBoardsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            (await getUserBoardsResponse.Content.ReadAsAsync<IEnumerable<BoardShortInfoResponse>>()).Should().BeEmpty();
        }
        
        [Fact]
        public async Task GetUserBoards_NotAuthenticatedRequest_ReturnsUnauthorized()
        {
            // Act
            var getUserBoardsResponse = await TestClient.GetAsync(ApiRoutes.Dashboards.GetUserBoards);

            // Assert
            getUserBoardsResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        
        [Fact]
        public async Task GetUserBoards_BoardsExist_ReturnsCorrectNumberOfBoards()
        {
            // Arrange 
            await AuthenticatedRequest();
            await TestClient.PostAsJsonAsync(ApiRoutes.Boards.CreateBoard, new CreateBoardRequest
            {
                Name = "Test Board",
                Description = "Test Board Description"
            });

            // Act
            var getUserBoardsResponse = await TestClient.GetAsync(ApiRoutes.Dashboards.GetUserBoards);

            // Assert
            getUserBoardsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            (await getUserBoardsResponse.Content.ReadAsAsync<IEnumerable<BoardShortInfoResponse>>()).Should()
                .NotBeEmpty();
        }
        
        // TODO: get-closed-user-boards
    }
}