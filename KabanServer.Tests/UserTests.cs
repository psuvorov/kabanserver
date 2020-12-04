using System;
using KabanServer.Data;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace KabanServer.Tests
{
    [TestFixture]
    public class UserTests
    {
        private DataContext _db;

        // TODO: ??
        // [TestFixtureSetup]
        // public void InitialSetup()
        // {
        //     
        // }
        
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
        
        
        
        
    }
}