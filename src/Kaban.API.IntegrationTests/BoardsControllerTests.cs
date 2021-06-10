using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Kaban.API.Controllers;
using Kaban.API.Controllers.Requests.Boards;
using Kaban.API.Controllers.Requests.CardComments;
using Kaban.API.Controllers.Requests.Cards;
using Kaban.API.Controllers.Requests.Lists;
using Kaban.API.Controllers.Responses;
using Kaban.API.Controllers.Responses.Boards;
using Kaban.API.Controllers.Responses.Cards;
using Kaban.API.Controllers.Responses.Lists;
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
        public async Task UpdateBoardInfo_ValidUpdateInfo_ReturnsOk()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.PutAsJsonAsync(ApiRoutes.Boards.UpdateBoardInfo, new UpdateBoardRequest
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
            getBoardResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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