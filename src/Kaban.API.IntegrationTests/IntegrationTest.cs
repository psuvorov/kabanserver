using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Kaban.API.Controllers;
using Kaban.API.Controllers.Requests.Users;
using Kaban.API.Controllers.Responses.Users;
using Kaban.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        
        // protected async Task AuthenticateAsync()
        // {
        //     _testClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await );
        // }

        // private async Task<string> GetJwtAsync()
        // {
        //     var response = await _testClient.PostAsJsonAsync(ApiRoutes.Users.RegisterUser, new RegisterUserDto
        //     {
        //         Email = "testuser@mail.com",
        //         Password = "TestUserP@ss"
        //     });
        //     
        //     var registrationResponse = await response.Content.ReadAsAsync<ObjectResult>();
        //     if (registrationResponse.StatusCode == StatusCodes.Status201Created)
        //     {
        //         
        //     }
        // }

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

        public void Dispose()
        {
            var serviceScope = _serviceProvider.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<DataContext>();
            context.Database.EnsureDeleted();
        }
    }
}