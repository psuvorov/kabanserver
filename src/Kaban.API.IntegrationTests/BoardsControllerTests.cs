using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Kaban.API.Controllers;
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
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.BoardPage.GetBoard.Replace("{boardId}", dummyBoard.BoardId.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var boardResponse = await response.Content.ReadAsAsync<BoardResponse>();
            boardResponse.Name.Should().NotBeNullOrEmpty();
            boardResponse.Description.Should().NotBeNullOrEmpty();
            boardResponse.Lists.Should().NotBeNullOrEmpty();
        }
        
        [Fact]
        public async Task GetList_CorrectBoardData_ReturnsListResponse()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.BoardPage.GetList
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{listId}", dummyBoard.ListId.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var listResponse = await response.Content.ReadAsAsync<ListResponse>();
            listResponse.Name.Should().NotBeNullOrEmpty();
            listResponse.OrderNumber.Should().BeGreaterThan(0);
            listResponse.Cards.Should().NotBeNullOrEmpty();
        }
        
        [Fact]
        public async Task GetBoardDetails_CorrectBoardData_ReturnsBoardDetailsResponse()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.BoardPage.GetBoardDetails.Replace("{boardId}", dummyBoard.BoardId.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var boardResponse = await response.Content.ReadAsAsync<BoardDetailsResponse>();
            boardResponse.Name.Should().NotBeNullOrEmpty();
            boardResponse.Description.Should().NotBeNullOrEmpty();
            boardResponse.Author.Id.Should().NotBeEmpty();
            boardResponse.Author.Email.Should().NotBeNullOrEmpty();
            boardResponse.Participants.Should().BeEmpty();
            boardResponse.Created.Should().HaveDay(DateTime.UtcNow.Day);
            boardResponse.Created.Should().HaveYear(DateTime.UtcNow.Year);
            boardResponse.LastModified.Should().BeNull();
        }
        
        [Fact]
        public async Task GetCardDetails_CorrectBoardData_ReturnsCardDetailsResponse()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.BoardPage.GetCardDetails
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{cardId}", dummyBoard.Card1Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var listResponse = await response.Content.ReadAsAsync<CardDetailsResponse>();
            listResponse.Name.Should().NotBeNullOrEmpty();
            listResponse.Description.Should().NotBeNullOrEmpty();
            listResponse.OrderNumber.Should().BeGreaterThan(0);
            listResponse.ListId.Should().NotBeEmpty();
            listResponse.ListName.Should().NotBeNullOrEmpty();
            listResponse.Comments.Should().BeEmpty();
        }

        [Fact]
        public async Task GetArchivedLists_CorrectBoardData_ReturnsEmptyArchivedLists()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var response = await TestClient.GetAsync(ApiRoutes.BoardPage.GetArchivedLists.Replace("{boardId}", dummyBoard.BoardId.ToString()));
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var archivedListResponse = await response.Content.ReadAsAsync<IEnumerable<ArchivedListResponse>>();
            archivedListResponse.Should().BeEmpty();
        }
        
        [Fact]
        public async Task GetArchivedCards_CorrectBoardData_ReturnsEmptyArchivedCards()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var response = await TestClient.GetAsync(ApiRoutes.BoardPage.GetArchivedCards.Replace("{boardId}", dummyBoard.BoardId.ToString()));
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var archivedCardResponse = await response.Content.ReadAsAsync<IEnumerable<ArchivedCardResponse>>();
            archivedCardResponse.Should().BeEmpty();
        }
    }
}