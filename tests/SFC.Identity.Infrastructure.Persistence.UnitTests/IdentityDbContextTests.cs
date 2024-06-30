using Microsoft.EntityFrameworkCore;

using SFC.Identity.Application.Common.Constants;

using Xunit;

namespace SFC.Identity.Infrastructure.Persistence.UnitTests;
public class IdentityDbContextTests
{
    private readonly DbContextOptions<IdentityDbContext> _dbContextOptions;

    public IdentityDbContextTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase($"IdentityDbContextTestsDb_{DateTime.Now.ToFileTimeUtc()}")
            .Options;
    }

    [Fact]
    [Trait("Persistence", "DbContext")]
    public void Persistence_DbContext_ShouldHasCorrectDefaultSchema()
    {
        IdentityDbContext context = new(_dbContextOptions);

        string? defaultSchema = context.Model.GetDefaultSchema();

        Assert.Equal(DbConstants.DEFAULT_SCHEMA_NAME, defaultSchema);
    }
}
