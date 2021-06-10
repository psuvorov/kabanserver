using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Kaban.API.Controllers;
using Kaban.API.Controllers.Requests.CardComments;
using Kaban.API.Controllers.Requests.Cards;
using Kaban.API.Controllers.Responses;
using Kaban.API.Controllers.Responses.Cards;
using Xunit;

namespace Kaban.API.IntegrationTests
{
    public class CardsControllerTests : IntegrationTest
    {
        [Fact]
        public async Task GetCardDetails_CorrectBoardData_ReturnsCardDetailsResponse()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Cards.GetCardDetails
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
        public async Task GetCardDetails_NonExistingBoardRequested_ReturnsNotFound()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Cards.GetCardDetails
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{cardId}", Guid.NewGuid().ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task GetArchivedCards_CorrectBoardData_ReturnsEmptyArchivedCards()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Cards.GetArchivedCards.Replace("{boardId}", dummyBoard.BoardId.ToString()));
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var archivedCardResponse = await response.Content.ReadAsAsync<IEnumerable<ArchivedCardResponse>>();
            archivedCardResponse.Should().BeEmpty();
        }
        
        [Fact]
        public async Task GetArchivedCards_NonExistingBoardRequested_ReturnsNotFound()
        {
            // Arrange
            await AuthenticatedRequest();
            
            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Cards.GetArchivedCards.Replace("{boardId}", Guid.NewGuid().ToString()));
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task CreateCard_CorrectCardData_ReturnsEntityCreatingSuccessResponse()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Cards.CreateCard, new CreateCardRequest
            {
                ListId = dummyBoard.List1Id,
                Name = "My New Test Card",
                Description = "My New Test Card Description",
                OrderNumber = 100
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var cardId = (await response.Content.ReadAsAsync<EntityCreatingSuccessResponse>()).EntityId;
            
            var getCardDetailsResponse = await TestClient.GetAsync(ApiRoutes.Cards.GetCardDetails
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{cardId}", cardId.ToString()));

            var cardResponse = await getCardDetailsResponse.Content.ReadAsAsync<CardDetailsResponse>();
            cardResponse.Name.Should().Be("My New Test Card");
            cardResponse.Description.Should().Be("My New Test Card Description");
            cardResponse.OrderNumber.Should().Be(100);
            cardResponse.Comments.Should().BeEmpty();
        }
        
        [Fact]
        public async Task CreateCard_NullCardName_ReturnsBadRequest()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Cards.CreateCard, new CreateCardRequest
            {
                ListId = dummyBoard.List1Id,
                Name = null,
                Description = "My New Test Card Description",
                OrderNumber = 100
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task CreateCard_EmptyCardName_ReturnsBadRequest()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Cards.CreateCard, new CreateCardRequest
            {
                ListId = dummyBoard.List1Id,
                Name = string.Empty,
                Description = "My New Test Card Description",
                OrderNumber = 100
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task CreateCard_NegativeOrderNumber_ReturnsBadRequest()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Cards.CreateCard, new CreateCardRequest
            {
                ListId = dummyBoard.List1Id,
                Name = "My New Test Card",
                Description = "My New Test Card Description",
                OrderNumber = -100
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task CreateCardComment_CorrectCardData_ReturnsEntityCreatingSuccessResponse()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Cards.CreateCardComment, new CreateCardCommentRequest
            {
                Text = "This is test comment",
                CardId = dummyBoard.Card1Id
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var cardCommentId = (await response.Content.ReadAsAsync<EntityCreatingSuccessResponse>()).EntityId;

            var getCardDetailsResponse = await TestClient.GetAsync(ApiRoutes.Cards.GetCardDetails
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{cardId}", dummyBoard.Card1Id.ToString()));

            var cardResponse = await getCardDetailsResponse.Content.ReadAsAsync<CardDetailsResponse>();
            cardCommentId.Should().NotBeEmpty();
            cardResponse.Comments.Should().NotBeEmpty();
            cardResponse.Comments.FirstOrDefault(x => x.Id == cardCommentId)?.Text.Should().Be("This is test comment");
        }
        
        [Fact]
        public async Task CreateCardComment_NullText_ReturnsBadRequest()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Cards.CreateCardComment, new CreateCardCommentRequest
            {
                Text = null,
                CardId = dummyBoard.Card1Id
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task CreateCardComment_EmptyText_ReturnsBadRequest()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Cards.CreateCardComment, new CreateCardCommentRequest
            {
                Text = string.Empty,
                CardId = dummyBoard.Card1Id
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task UpdateCard_ValidUpdateInfo_ReturnsOk()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.PutAsJsonAsync(ApiRoutes.Cards.UpdateCard, new UpdateCardRequest
            {
                CardId = dummyBoard.Card1Id,
                Name = "New Card 1",
                Description = "New Card 1 description",
                OrderNumber = 100
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getCardDetailsResponse = await TestClient.GetAsync(ApiRoutes.Cards.GetCardDetails
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{cardId}", dummyBoard.Card1Id.ToString()));
            getCardDetailsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var cardDetailsResponse = await getCardDetailsResponse.Content.ReadAsAsync<CardDetailsResponse>();
            cardDetailsResponse.Name.Should().Be("New Card 1");
            cardDetailsResponse.Description.Should().Be("New Card 1 description");
            cardDetailsResponse.OrderNumber.Should().Be(100);
        }
        
        [Fact]
        public async Task UpdateCard_NullNamePropertyValue_PreviousValueStays()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.PutAsJsonAsync(ApiRoutes.Cards.UpdateCard, new UpdateCardRequest
            {
                CardId = dummyBoard.Card1Id,
                Name = null,
                Description = "New Card 1 description",
                OrderNumber = 100
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getCardDetailsResponse = await TestClient.GetAsync(ApiRoutes.Cards.GetCardDetails
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{cardId}", dummyBoard.Card1Id.ToString()));
            getCardDetailsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var cardDetailsResponse = await getCardDetailsResponse.Content.ReadAsAsync<CardDetailsResponse>();
            cardDetailsResponse.Name.Should().Be("Card 1");
        }
        
        [Fact]
        public async Task UpdateCard_EmptyNamePropertyValue_PreviousValueStays()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.PutAsJsonAsync(ApiRoutes.Cards.UpdateCard, new UpdateCardRequest
            {
                CardId = dummyBoard.Card1Id,
                Name = string.Empty,
                Description = "New Card 1 description",
                OrderNumber = 100
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getCardDetailsResponse = await TestClient.GetAsync(ApiRoutes.Cards.GetCardDetails
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{cardId}", dummyBoard.Card1Id.ToString()));
            getCardDetailsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var cardDetailsResponse = await getCardDetailsResponse.Content.ReadAsAsync<CardDetailsResponse>();
            cardDetailsResponse.Name.Should().Be("Card 1");
        }
        
        [Fact]
        public async Task UpdateCard_NegativeOrderNumber_ReturnsBadRequest()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.PutAsJsonAsync(ApiRoutes.Cards.UpdateCard, new UpdateCardRequest
            {
                CardId = dummyBoard.Card1Id,
                Name = "New Card 1",
                Description = "New Card 1 description",
                OrderNumber = -100
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task ReorderCards_ValidUpdateInfo_CorrectUpdates()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.PutAsJsonAsync(ApiRoutes.Cards.ReorderCards, new List<ReorderCardRequest>
            {
                new ReorderCardRequest
                {
                    CardId = dummyBoard.Card1Id, 
                    OrderNumber = 10
                },
                new ReorderCardRequest
                {
                    CardId = dummyBoard.Card2Id, 
                    OrderNumber = 20
                },
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getCard1Response = await TestClient.GetAsync(ApiRoutes.Cards.GetCardDetails
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{cardId}", dummyBoard.Card1Id.ToString()));
            var getCard2Response = await TestClient.GetAsync(ApiRoutes.Cards.GetCardDetails
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{cardId}", dummyBoard.Card2Id.ToString()));
            
            var card1Response = await getCard1Response.Content.ReadAsAsync<CardDetailsResponse>();
            var card2Response = await getCard2Response.Content.ReadAsAsync<CardDetailsResponse>();
            card1Response.OrderNumber.Should().Be(10);
            card2Response.OrderNumber.Should().Be(20);
        }
        
        [Fact]
        public async Task ReorderCards_NegativeOrderNumber_ReturnsBadRequest()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.PutAsJsonAsync(ApiRoutes.Cards.ReorderCards, new List<ReorderCardRequest>
            {
                new ReorderCardRequest
                {
                    CardId = dummyBoard.Card1Id, 
                    OrderNumber = -10
                },
                new ReorderCardRequest
                {
                    CardId = dummyBoard.Card2Id, 
                    OrderNumber = -20
                },
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task DeleteCard_ExistingCard_DeletedSuccessfully()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var deleteResponse = await TestClient.DeleteAsync(ApiRoutes.Cards.DeleteCard
                .Replace("{cardId}", dummyBoard.Card1Id.ToString()));
            
            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var getCardDetailsResponse = await TestClient.GetAsync(ApiRoutes.Cards.GetCardDetails
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{cardId}", dummyBoard.Card1Id.ToString()));
            getCardDetailsResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task DeleteCard_NonExistingCard_ReturnsNoContent()
        {
            // Arrange
            await AuthenticatedRequest();
            var cardId = Guid.NewGuid();
            
            // Act
            var deleteResponse = await TestClient.DeleteAsync(ApiRoutes.Cards.DeleteCard
                .Replace("{cardId}", cardId.ToString()));
            
            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        
        [Fact]
        public async Task DeleteCardComment_ExistingCardComment_DeletedSuccessfully()
        {
            // Arrange
            await AuthenticatedRequest();
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var deleteResponse = await TestClient.DeleteAsync(ApiRoutes.Cards.DeleteCardComment
                .Replace("{cardCommentId}", dummyBoard.CardComment1Id.ToString()));
            
            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var getCardDetailsResponse = await TestClient.GetAsync(ApiRoutes.Cards.GetCardDetails
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{cardId}", dummyBoard.Card1Id.ToString()));
            var cardDetailsResponse = await getCardDetailsResponse.Content.ReadAsAsync<CardDetailsResponse>();
            cardDetailsResponse.Comments.Should().BeEmpty();
        }
        
        [Fact]
        public async Task DeleteCardComment_NonExistingCardComment_ReturnsNoContent()
        {
            // Arrange
            await AuthenticatedRequest();
            var cardCommentId = Guid.NewGuid();
            
            // Act
            var deleteResponse = await TestClient.DeleteAsync(ApiRoutes.Cards.DeleteCardComment
                .Replace("{cardCommentId}", cardCommentId.ToString()));
            
            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}