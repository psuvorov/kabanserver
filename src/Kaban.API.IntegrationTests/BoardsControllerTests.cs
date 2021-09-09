using System;
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
    public class BoardsControllerTests : IntegrationTest
    {
        [Fact]
        public async Task GetBoard_CorrectBoardData_ReturnsFullBoardInformation()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Boards.GetBoard.Replace("{boardId}", dummyBoard.BoardId.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var boardResponse = await response.Content.ReadAsAsync<BoardResponse>();
            boardResponse.Name.Should().NotBeNullOrEmpty();
            boardResponse.Description.Should().NotBeNullOrEmpty();
            boardResponse.Lists.Should().NotBeNullOrEmpty();
        }
        
        [Fact]
        public async Task GetBoard_NonExistingBoardRequested_ReturnsNotFound()
        {
            // Arrange
            await AuthenticatedRequest();

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Boards.GetBoard.Replace("{boardId}", Guid.NewGuid().ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task GetBoardDetails_CorrectBoardData_ReturnsBoardDetailsResponse()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Boards.GetBoardDetails.Replace("{boardId}", dummyBoard.BoardId.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var boardResponse = await response.Content.ReadAsAsync<BoardDetailsResponse>();
            boardResponse.Name.Should().NotBeNullOrEmpty();
            boardResponse.Description.Should().NotBeNullOrEmpty();
            boardResponse.Author.Id.Should().NotBeEmpty();
            boardResponse.Author.Email.Should().NotBeNullOrEmpty();
            boardResponse.Participants.Should().BeEmpty();
            boardResponse.Created.Should().HaveYear(DateTime.UtcNow.Year); // Do not run this test on the New Year day ;)
            boardResponse.LastModified.Should().BeNull();
        }
        
        [Fact]
        public async Task GetBoardDetails_NonExistingBoardRequested_ReturnsNotFound()
        {
            // Arrange
            await AuthenticatedRequest();

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Boards.GetBoardDetails.Replace("{boardId}", Guid.NewGuid().ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task CreateBoard_CorrectBoardData_ReturnsCreatedBoardData()
        {
            // Arrange 
            await AuthenticatedRequest();
            
            // Act
            await TestClient.PostAsJsonAsync(ApiRoutes.Boards.CreateBoard, new CreateBoardRequest
            {
                Name = "Test Board",
                Description = "Test Board Description"
            });

            // Assert
            var getUserBoardsResponse = await TestClient.GetAsync(ApiRoutes.Dashboards.GetUserBoards);
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
            var createBoardResponse = await TestClient.PostAsJsonAsync(ApiRoutes.Boards.CreateBoard, new CreateBoardRequest());

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
            await TestClient.PostAsJsonAsync(ApiRoutes.Boards.CreateBoard, createBoardRequest);
            var createBoardResponse = await TestClient.PostAsJsonAsync(ApiRoutes.Boards.CreateBoard, createBoardRequest);

            // Assert
            createBoardResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task UpdateBoardInfo_ValidUpdateInfo_ReturnsOk()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.PutAsJsonAsync(ApiRoutes.Boards.UpdateBoardInfo.Replace("{boardId}", dummyBoard.BoardId.ToString()), new UpdateBoardRequest
            {
                BoardId = dummyBoard.BoardId,
                Name = "Test New Board",
                Description = "Test New Board Description"
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getBoardResponse = await TestClient.GetAsync(ApiRoutes.Boards.GetBoard.Replace("{boardId}", dummyBoard.BoardId.ToString()));
            getBoardResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var boardResponse = await getBoardResponse.Content.ReadAsAsync<BoardResponse>();
            boardResponse.Name.Should().Be("Test New Board");
            boardResponse.Description.Should().Be("Test New Board Description");
        }
        
        [Fact]
        public async Task UpdateBoardInfo_NullNamePropertyValue_PreviousValueStays()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.PutAsJsonAsync(ApiRoutes.Boards.UpdateBoardInfo.Replace("{boardId}", dummyBoard.BoardId.ToString()), new UpdateBoardRequest
            {
                BoardId = dummyBoard.BoardId,
                Name = null,
                Description = "Test New Board Description"
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getBoardResponse = await TestClient.GetAsync(ApiRoutes.Boards.GetBoard.Replace("{boardId}", dummyBoard.BoardId.ToString()));
            getBoardResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var boardResponse = await getBoardResponse.Content.ReadAsAsync<BoardResponse>();
            boardResponse.Name.Should().Be("Test Board");
        }
        
        [Fact]
        public async Task UpdateBoardInfo_EmptyNamePropertyValue_PreviousValueStays()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.PutAsJsonAsync(ApiRoutes.Boards.UpdateBoardInfo.Replace("{boardId}", dummyBoard.BoardId.ToString()), new UpdateBoardRequest
            {
                BoardId = dummyBoard.BoardId,
                Name = string.Empty,
                Description = "Test New Board Description"
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getBoardResponse = await TestClient.GetAsync(ApiRoutes.Boards.GetBoard.Replace("{boardId}", dummyBoard.BoardId.ToString()));
            getBoardResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var boardResponse = await getBoardResponse.Content.ReadAsAsync<BoardResponse>();
            boardResponse.Name.Should().Be("Test Board");
        }

        [Fact]
        public async Task DeleteBoard_ExistingBoard_DeletedSuccessfully()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var deleteResponse = await TestClient.DeleteAsync(ApiRoutes.Boards.DeleteBoard
                .Replace("{boardId}", dummyBoard.BoardId.ToString()));
            
            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var getBoardResponse = await TestClient.GetAsync(ApiRoutes.Boards.GetBoard.Replace("{boardId}", dummyBoard.BoardId.ToString()));
            getBoardResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task DeleteBoard_NonExistingBoard_ReturnsNoContent()
        {
            // Arrange
            await AuthenticatedRequest();
            var boardId = Guid.NewGuid();
            
            // Act
            var deleteResponse = await TestClient.DeleteAsync(ApiRoutes.Boards.DeleteBoard
                .Replace("{boardId}", boardId.ToString()));
            
            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}