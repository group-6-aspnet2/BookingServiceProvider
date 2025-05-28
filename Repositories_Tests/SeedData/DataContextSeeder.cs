using Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Repositories_Tests.SeedData;

public class DataContextSeeder
{
    public DataContext GetDataContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new DataContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
