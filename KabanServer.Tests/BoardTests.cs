using System;
using System.Threading.Tasks;
using KabanServer.Data;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace KabanServer.Tests
{
    
    public class BoardTests
    {
        private DataContext _db;
        
        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _db = new DataContext(options);
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }
        
        //public async Task Create

        
    }
}