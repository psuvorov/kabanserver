using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Kaban.API.Controllers;
using Kaban.API.Controllers.Requests.Lists;
using Kaban.API.Controllers.Responses;
using Kaban.API.Controllers.Responses.Lists;
using Xunit;

namespace Kaban.API.IntegrationTests
{
    public class ListsControllerTests : IntegrationTest
    {
        [Fact]
        public async Task GetList_CorrectBoardData_ReturnsListResponse()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Lists.GetList
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
        public async Task GetArchivedLists_CorrectBoardData_ReturnsEmptyArchivedLists()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Lists.GetArchivedLists.Replace("{boardId}", dummyBoard.BoardId.ToString()));
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var archivedListResponse = await response.Content.ReadAsAsync<IEnumerable<ArchivedListResponse>>();
            archivedListResponse.Should().BeEmpty();
        }
        
        [Fact]
        public async Task CreateList_CorrectListData_ReturnsEntityCreatingSuccessResponse()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Lists.CreateList, new CreateListRequest
            {
                BoardId = dummyBoard.BoardId,
                Name = "My New Test List",
                OrderNumber = 100
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var listId = (await response.Content.ReadAsAsync<EntityCreatingSuccessResponse>()).EntityId;
            var getListResponse = await TestClient.GetAsync(ApiRoutes.Lists.GetList
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{listId}", listId.ToString()));

            var listResponse = await getListResponse.Content.ReadAsAsync<ListResponse>();
            listResponse.Name.Should().Be("My New Test List");
            listResponse.OrderNumber.Should().Be(100);
            listResponse.Cards.Should().BeEmpty();
        }
        
        [Fact]
        public async Task CopyList_CorrectListData_ReturnsEntityCreatingSuccessResponse()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Lists.CopyList, new CopyListRequest
            {
                BoardId = dummyBoard.BoardId,
                ListId = dummyBoard.List1Id
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var copiedListId = (await response.Content.ReadAsAsync<EntityCreatingSuccessResponse>()).EntityId;
            var getListResponse = await TestClient.GetAsync(ApiRoutes.Lists.GetList
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{listId}", copiedListId.ToString()));

            var listResponse = await getListResponse.Content.ReadAsAsync<ListResponse>();
            listResponse.Name.Should().Be("List 1");
            listResponse.OrderNumber.Should().Be(1);
            listResponse.Cards.Should().NotBeEmpty();
        }
        
        [Fact]
        public async Task UpdateList_ValidUpdateInfo_ReturnsOk()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.PutAsJsonAsync(ApiRoutes.Lists.UpdateList, new UpdateListRequest
            {
                ListId = dummyBoard.List1Id,
                Name = "New List 1",
                OrderNumber = 100
            });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getListResponse = await TestClient.GetAsync(ApiRoutes.Lists.GetList
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{listId}", dummyBoard.List1Id.ToString()));
            getListResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var listResponse = await getListResponse.Content.ReadAsAsync<ListResponse>();
            listResponse.Name.Should().Be("New List 1");
            listResponse.OrderNumber.Should().Be(100);
        }
        
        [Fact]
        public async Task RenumberLists_ValidUpdateInfo_CorrectUpdates()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();

            // Act
            var response = await TestClient.PutAsJsonAsync(ApiRoutes.Lists.RenumberLists, new List<RenumberListRequest>
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
            var getList1Response = await TestClient.GetAsync(ApiRoutes.Lists.GetList
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{listId}", dummyBoard.List1Id.ToString()));
            var getList2Response = await TestClient.GetAsync(ApiRoutes.Lists.GetList
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{listId}", dummyBoard.List2Id.ToString()));

            var list1Response = await getList1Response.Content.ReadAsAsync<ListResponse>();
            var list2Response = await getList2Response.Content.ReadAsAsync<ListResponse>();
            list1Response.OrderNumber.Should().Be(10);
            list2Response.OrderNumber.Should().Be(20);
        }
        
        [Fact]
        public async Task DeleteList_ExistingList_DeletedSuccessfully()
        {
            // Arrange
            var dummyBoard = await CreateDummyBoard();
            
            // Act
            var deleteResponse = await TestClient.DeleteAsync(ApiRoutes.Lists.DeleteList
                .Replace("{listId}", dummyBoard.List1Id.ToString()));
            
            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var getListResponse = await TestClient.GetAsync(ApiRoutes.Lists.GetList
                .Replace("{boardId}", dummyBoard.BoardId.ToString())
                .Replace("{listId}", dummyBoard.List1Id.ToString()));
            getListResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}