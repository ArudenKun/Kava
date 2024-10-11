using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Kava.Core.Data;

[RequiresDynamicCode("Calls IDesignTimeDbContextFactory<T>.CreateDbContext")]
[RequiresUnreferencedCode("Calls IDesignTimeDbContextFactory<T>.CreateDbContext")]
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlite("DataSource=:memory:");

        return new AppDbContext(optionsBuilder.Options);
    }
}
