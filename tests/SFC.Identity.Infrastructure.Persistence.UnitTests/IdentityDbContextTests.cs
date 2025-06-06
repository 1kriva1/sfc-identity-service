﻿using Microsoft.EntityFrameworkCore;

using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Infrastructure.Persistence.Constants;
using SFC.Identity.Infrastructure.Persistence.Contexts;

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

    //[Fact]
    //[Trait("Persistence", "DbContext")]
    //public void Persistence_DbContext_ShouldHasCorrectDefaultSchema()
    //{
    //    IdentityDbContext context = new(_dbContextOptions);

    //    string? defaultSchema = context.Model.GetDefaultSchema();

    //    Assert.Equal(DatabaseConstants.DEFAULT_SCHEMA_NAME, defaultSchema);
    //}
}