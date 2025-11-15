using Microsoft.EntityFrameworkCore;
using StudentPerformanceTracker.Data.Context;

namespace StudentPerformanceTracker.Tests
{
    public class TestBase : IDisposable
    {
        protected ApplicationDbContext Context { get; private set; }

        public TestBase()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Context = new ApplicationDbContext(options);
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}