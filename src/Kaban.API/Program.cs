using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kaban.Domain.Interfaces;
using Kaban.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kaban.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            try
            {
                using var scope = host.Services.CreateScope();
                var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                var userService  = scope.ServiceProvider.GetRequiredService<IUserService>();
                var boardService  = scope.ServiceProvider.GetRequiredService<IBoardService>();
                

                if (env.IsDevelopment())
                {
                    var user1 = new User()
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "user1",
                        LastName = "user1",
                        Email = "user1@mail.ru",
                        
                    };
                    userService.Create(user1, "123M@aaaaaa").GetAwaiter().GetResult();


                    var board1 = new Board
                    {
                        Id = Guid.NewGuid(),
                        Name = "Board1",
                        Description = "Board1"
                    };

                    boardService.Create(board1);


                    var board = boardService.Get(board1.Id);
                    var d = 5;

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseWebRoot("Content");
                    webBuilder.UseStartup<Startup>();
                });
    }
}