using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using Kaban.API.Controllers;
using Kaban.API.Controllers.Requests.Users;
using Kaban.API.Controllers.Responses;
using Kaban.API.Controllers.Responses.Boards;
using Kaban.API.Controllers.Responses.Users;
using Xunit;

namespace Kaban.API.IntegrationTests
{
    public class DashboardControllerTests : IntegrationTest
    {
        [Fact]
        public async Task GetAllUserBoards_NoneOfBoardsExist_ReturnsEmptyEnumerable()
        {
            // Arrange 
            await AuthenticatedRequest();

            // Act
            var getUserBoardsResponse = await TestClient.GetAsync(ApiRoutes.Dashboard.GetUserBoards);

            // Assert
            getUserBoardsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            (await getUserBoardsResponse.Content.ReadAsAsync<IEnumerable<BoardShortInfoResponse>>()).Should().BeEmpty();
        }
    }
}