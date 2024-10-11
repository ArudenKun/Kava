using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Kava.Data;

[RequiresUnreferencedCode("Uses unreferenced code")]
[RequiresDynamicCode("Uses dynamic code")]
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlite(@"Data Source=C:\Users\alden\AppData\Roaming\Kava\data.debug.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}
