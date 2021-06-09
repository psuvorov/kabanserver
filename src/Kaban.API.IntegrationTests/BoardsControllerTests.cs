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
                .Replace("{listId}", dummyBoard.List1Id.ToString()));

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
            listResponse.Comments.Should().NotBeEmpty();
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

        [Fact]
        public async Task CreateList_CorrectListData_ReturnsEntityCreatingSuccessResponse()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.BoardPage.CreateList, new CreateListRequest
            {
                BoardId = dummyBoard.BoardId,
                Name = "My New Test List",
                OrderNumber = 100
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var listId = (await response.Content.ReadAsAsync<EntityCreatingSuccessResponse>()).EntityId;
            var getListResponse = await TestClient.GetAsync(ApiRoutes.BoardPage.GetList
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{listId}", listId.ToString()));

            var listResponse = await getListResponse.Content.ReadAsAsync<ListResponse>();
            listResponse.Name.Should().Be("My New Test List");
            listResponse.OrderNumber.Should().Be(100);
            listResponse.Cards.Should().BeEmpty();
        }
        
        [Fact]
        public async Task CreateCard_CorrectCardData_ReturnsEntityCreatingSuccessResponse()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.BoardPage.CreateCard, new CreateCardRequest
            {
                ListId = dummyBoard.List1Id,
                Name = "My New Test Card",
                Description = "My New Test Card Description",
                OrderNumber = 100
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var cardId = (await response.Content.ReadAsAsync<EntityCreatingSuccessResponse>()).EntityId;
            
            var getCardDetailsResponse = await TestClient.GetAsync(ApiRoutes.BoardPage.GetCardDetails
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{cardId}", cardId.ToString()));

            var cardResponse = await getCardDetailsResponse.Content.ReadAsAsync<CardDetailsResponse>();
            cardResponse.Name.Should().Be("My New Test Card");
            cardResponse.Description.Should().Be("My New Test Card Description");
            cardResponse.OrderNumber.Should().Be(100);
            cardResponse.Comments.Should().BeEmpty();
        }
        
        [Fact]
        public async Task CreateCardComment_CorrectCardData_ReturnsEntityCreatingSuccessResponse()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.BoardPage.CreateCardComment, new CreateCardCommentRequest
            {
                Text = "This is test comment",
                CardId = dummyBoard.Card1Id
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var cardCommentId = (await response.Content.ReadAsAsync<EntityCreatingSuccessResponse>()).EntityId;

            var getCardDetailsResponse = await TestClient.GetAsync(ApiRoutes.BoardPage.GetCardDetails
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{cardId}", dummyBoard.Card1Id.ToString()));

            var cardResponse = await getCardDetailsResponse.Content.ReadAsAsync<CardDetailsResponse>();
            cardCommentId.Should().NotBeEmpty();
            cardResponse.Comments.Should().NotBeEmpty();
            cardResponse.Comments.FirstOrDefault(x => x.Id == cardCommentId)?.Text.Should().Be("This is test comment");
        }
        
        [Fact]
        public async Task CopyList_CorrectListData_ReturnsEntityCreatingSuccessResponse()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.BoardPage.CopyList, new CopyListRequest
            {
                BoardId = dummyBoard.BoardId,
                ListId = dummyBoard.List1Id
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var copiedListId = (await response.Content.ReadAsAsync<EntityCreatingSuccessResponse>()).EntityId;
            var getListResponse = await TestClient.GetAsync(ApiRoutes.BoardPage.GetList
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{listId}", copiedListId.ToString()));

            var listResponse = await getListResponse.Content.ReadAsAsync<ListResponse>();
            listResponse.Name.Should().Be("List 1");
            listResponse.OrderNumber.Should().Be(1);
            listResponse.Cards.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UpdateBoardInfo_ValidUpdateInfo_ReturnsOk()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.PutAsJsonAsync(ApiRoutes.BoardPage.UpdateBoardInfo, new UpdateBoardRequest
            {
                BoardId = dummyBoard.BoardId,
                Name = "Test New Board",
                Description = "Test New Board Description"
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getBoardResponse = await TestClient.GetAsync(ApiRoutes.BoardPage.GetBoard.Replace("{boardId}", dummyBoard.BoardId.ToString()));
            getBoardResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var boardResponse = await getBoardResponse.Content.ReadAsAsync<BoardResponse>();
            boardResponse.Name.Should().Be("Test New Board");
            boardResponse.Description.Should().Be("Test New Board Description");
        }
        
        [Fact]
        public async Task UpdateList_ValidUpdateInfo_ReturnsOk()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.PutAsJsonAsync(ApiRoutes.BoardPage.UpdateList, new UpdateListRequest
            {
                ListId = dummyBoard.List1Id,
                Name = "New List 1",
                OrderNumber = 100
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getListResponse = await TestClient.GetAsync(ApiRoutes.BoardPage.GetList
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{listId}", dummyBoard.List1Id.ToString()));
            getListResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var listResponse = await getListResponse.Content.ReadAsAsync<ListResponse>();
            listResponse.Name.Should().Be("New List 1");
            listResponse.OrderNumber.Should().Be(100);
        }
        
        [Fact]
        public async Task UpdateCard_ValidUpdateInfo_ReturnsOk()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.PutAsJsonAsync(ApiRoutes.BoardPage.UpdateCard, new UpdateCardRequest
            {
                CardId = dummyBoard.Card1Id,
                Name = "New Card 1",
                Description = "New Card 1 description",
                OrderNumber = 100
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getCardDetailsResponse = await TestClient.GetAsync(ApiRoutes.BoardPage.GetCardDetails
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{cardId}", dummyBoard.Card1Id.ToString()));
            getCardDetailsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var cardDetailsResponse = await getCardDetailsResponse.Content.ReadAsAsync<CardDetailsResponse>();
            cardDetailsResponse.Name.Should().Be("New Card 1");
            cardDetailsResponse.Description.Should().Be("New Card 1 description");
            cardDetailsResponse.OrderNumber.Should().Be(100);
        }
        
        [Fact]
        public async Task RenumberLists_ValidUpdateInfo_CorrectUpdates()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.PutAsJsonAsync(ApiRoutes.BoardPage.RenumberLists, new List<RenumberListRequest>
            {
                new RenumberListRequest
                {
                    ListId = dummyBoard.List1Id,
                    OrderNumber = 10
                },
                new RenumberListRequest
                {
                    ListId = dummyBoard.List2Id,
                    OrderNumber = 20
                }
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getList1Response = await TestClient.GetAsync(ApiRoutes.BoardPage.GetList
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{listId}", dummyBoard.List1Id.ToString()));
            var getList2Response = await TestClient.GetAsync(ApiRoutes.BoardPage.GetList
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{listId}", dummyBoard.List2Id.ToString()));

            var list1Response = await getList1Response.Content.ReadAsAsync<ListResponse>();
            var list2Response = await getList2Response.Content.ReadAsAsync<ListResponse>();
            list1Response.OrderNumber.Should().Be(10);
            list2Response.OrderNumber.Should().Be(20);
        }
        
        [Fact]
        public async Task RenumberCards_ValidUpdateInfo_CorrectUpdates()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.PutAsJsonAsync(ApiRoutes.BoardPage.RenumberCards, new List<RenumberCardRequest>
            {
                new RenumberCardRequest
                {
                    CardId = dummyBoard.Card1Id, 
                    OrderNumber = 10
                },
                new RenumberCardRequest
                {
                    CardId = dummyBoard.Card2Id, 
                    OrderNumber = 20
                },
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getCard1Response = await TestClient.GetAsync(ApiRoutes.BoardPage.GetCardDetails
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{cardId}", dummyBoard.Card1Id.ToString()));
            var getCard2Response = await TestClient.GetAsync(ApiRoutes.BoardPage.GetCardDetails
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{cardId}", dummyBoard.Card2Id.ToString()));
            
            var card1Response = await getCard1Response.Content.ReadAsAsync<CardDetailsResponse>();
            var card2Response = await getCard2Response.Content.ReadAsAsync<CardDetailsResponse>();
            card1Response.OrderNumber.Should().Be(10);
            card2Response.OrderNumber.Should().Be(20);
        }

        [Fact]
        public async Task DeleteBoard_ExistingBoard_DeletedSuccessfully()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var deleteResponse = await TestClient.DeleteAsync(ApiRoutes.BoardPage.DeleteBoard
                .Replace("{boardId}", dummyBoard.BoardId.ToString()));
            
            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var getBoardResponse = await TestClient.GetAsync(ApiRoutes.BoardPage.GetBoard.Replace("{boardId}", dummyBoard.BoardId.ToString()));
            getBoardResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task DeleteList_ExistingList_DeletedSuccessfully()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var deleteResponse = await TestClient.DeleteAsync(ApiRoutes.BoardPage.DeleteList
                .Replace("{listId}", dummyBoard.List1Id.ToString()));
            
            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var getListResponse = await TestClient.GetAsync(ApiRoutes.BoardPage.GetList
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{listId}", dummyBoard.List1Id.ToString()));
            getListResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task DeleteCard_ExistingCard_DeletedSuccessfully()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var deleteResponse = await TestClient.DeleteAsync(ApiRoutes.BoardPage.DeleteCard
                .Replace("{cardId}", dummyBoard.Card1Id.ToString()));
            
            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var getCardDetailsResponse = await TestClient.GetAsync(ApiRoutes.BoardPage.GetCardDetails
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{cardId}", dummyBoard.Card1Id.ToString()));
            getCardDetailsResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task DeleteCardComment_ExistingCardComment_DeletedSuccessfully()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var deleteResponse = await TestClient.DeleteAsync(ApiRoutes.BoardPage.DeleteCardComment
                .Replace("{cardCommentId}", dummyBoard.CardComment1Id.ToString()));
            
            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var getCardDetailsResponse = await TestClient.GetAsync(ApiRoutes.BoardPage.GetCardDetails
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{cardId}", dummyBoard.Card1Id.ToString()));
            var cardDetailsResponse = await getCardDetailsResponse.Content.ReadAsAsync<CardDetailsResponse>();
            cardDetailsResponse.Comments.Should().BeEmpty();
        }
        
    }
}