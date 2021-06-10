using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Kaban.API.Controllers;
using Kaban.API.Controllers.Requests.Boards;
using Kaban.API.Controllers.Requests.CardComments;
using Kaban.API.Controllers.Requests.Cards;
using Kaban.API.Controllers.Requests.Lists;
using Kaban.API.Controllers.Requests.Users;
using Kaban.API.Controllers.Responses;
using Kaban.API.Controllers.Responses.Users;
using Kaban.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kaban.API.IntegrationTests
{
    public class IntegrationTest : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        
        protected readonly HttpClient TestClient;

        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(DataContext));
                        services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase("TestDb"));
                    });
                });
            _serviceProvider = appFactory.Services;
            TestClient = appFactory.CreateClient();
        }
        
        protected async Task<HttpResponseMessage> RegisterUserAsync(RegisterUserRequest request)
        {
            return await TestClient.PostAsJsonAsync(ApiRoutes.Users.RegisterUser, request);
        }

        protected async Task<HttpResponseMessage> AuthenticateAsync(AuthenticateRequest request)
        {
            return await TestClient.PostAsJsonAsync(ApiRoutes.Users.AuthenticateUser, request);
        }

        protected async Task AuthenticatedRequest()
        {
            var registerUser = new RegisterUserRequest
            {
                FirstName = "user1",
                LastName = "user1",
                Username = "user1",
                Email = "user1@mail.com",
                Password = "UsEr1"
            };
            await RegisterUserAsync(registerUser);
            var authRequest = new AuthenticateRequest
            {
                Email = registerUser.Email,
                Password = registerUser.Password
            };
            var authenticateResponse = await AuthenticateAsync(authRequest);
            var jwt = (await authenticateResponse.Content.ReadAsAsync<AuthSuccessResponse>());
            
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", jwt.Token);
        }

        protected async Task<DummyBoard> CreateDummyBoard()
        {
            await AuthenticatedRequest();

            var createBoardResponse = await TestClient.PostAsJsonAsync(ApiRoutes.Dashboard.CreateBoard,
                new CreateBoardRequest
                {
                    Name = "Test Board",
                    Description = "Test Board Description"
                });
            var boardId = (await createBoardResponse.Content.ReadAsAsync<EntityCreatingSuccessResponse>()).EntityId;

            var createList1Response = await TestClient.PostAsJsonAsync(ApiRoutes.Lists.CreateList,
                new CreateListRequest
                {
                    BoardId = boardId,
                    Name = "List 1",
                    OrderNumber = 1
                });
            var list1Id = (await createList1Response.Content.ReadAsAsync<EntityCreatingSuccessResponse>()).EntityId;

            var createList2Response = await TestClient.PostAsJsonAsync(ApiRoutes.Lists.CreateList,
                new CreateListRequest
                {
                    BoardId = boardId,
                    Name = "List 2",
                    OrderNumber = 2
                });
            var list2Id = (await createList2Response.Content.ReadAsAsync<EntityCreatingSuccessResponse>()).EntityId;

            var createCard1Response = await TestClient.PostAsJsonAsync(ApiRoutes.Cards.CreateCard,
                new CreateCardRequest
                {
                    ListId = list1Id,
                    Name = "Card 1",
                    Description = "Card 1 description",
                    OrderNumber = 1
                });
            var card1Id = (await createCard1Response.Content.ReadAsAsync<EntityCreatingSuccessResponse>()).EntityId;

            var createCardComment1Response = await TestClient.PostAsJsonAsync(ApiRoutes.Cards.CreateCardComment, new CreateCardCommentRequest
            {
                CardId = card1Id,
                Text = "This is the test card comment"
            });
            var cardComment1Id = (await createCardComment1Response.Content.ReadAsAsync<EntityCreatingSuccessResponse>()).EntityId;

            var createCard2Response = await TestClient.PostAsJsonAsync(ApiRoutes.Cards.CreateCard, new CreateCardRequest
            {
                ListId = list1Id,
                Name = "Card 2",
                Description = "Card 2 description",
                OrderNumber = 2
            });
            var card2Id = (await createCard2Response.Content.ReadAsAsync<EntityCreatingSuccessResponse>()).EntityId;

            return new DummyBoard
            {
                BoardId = boardId,
                List1Id = list1Id,
                List2Id = list2Id,
                Card1Id = card1Id,
                Card2Id = card2Id,
                CardComment1Id = cardComment1Id
            };
        }

        public void Dispose()
        {
            var serviceScope = _serviceProvider.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<DataContext>();
            context.Database.EnsureDeleted();
        }
        
        protected class DummyBoard
        {
            public Guid BoardId { get; set; }
            public Guid List1Id { get; set; }
            public Guid List2Id { get; set; }
            public Guid Card1Id { get; set; }
            public Guid Card2Id { get; set; }
            public Guid CardComment1Id { get; set; }
        }
    }
}