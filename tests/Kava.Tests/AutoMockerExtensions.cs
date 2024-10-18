// using Kava.Data;
// using Kava.Data.Interceptors;
// using Microsoft.Data.Sqlite;
// using Microsoft.EntityFrameworkCore;
// using Moq.AutoMock.Resolvers;
//
// namespace Kava.Tests;
//
// public static class AutoMockerExtensions
// {
//     public static IDbScope<AppDbContext> WithDbScope(this AutoMocker mocker)
//     {
//         var resolver = new DbScopedResolver();
//         var existing = mocker.Resolvers.ToList();
//         mocker.Resolvers.Clear();
//         existing.Insert(0, resolver);
//         existing.Add(resolver);
//         foreach (var existingResolver in existing)
//         {
//             mocker.Resolvers.Add(existingResolver);
//         }
//         return resolver;
//     }
//
//     public interface IDbScope<TContext> : IDbContextFactory<TContext>, IDisposable
//         where TContext : DbContext { }
//
//     private sealed class DbScopedResolver : IMockResolver, IDbScope<AppDbContext>
//     {
//         private bool _disposedValue;
//
//         private readonly Lazy<SqliteConnection> _sqliteConnection =
//             new(() =>
//             {
//                 var connection = new SqliteConnection("DataSource=:memory:");
//                 connection.Open();
//                 return connection;
//             });
//
//         public void Resolve(MockResolutionContext context)
//         {
//             if (context.RequestType == typeof(AppDbContext))
//                 context.Value = CreateDbContext();
//             else if (context.RequestType == typeof(Func<AppDbContext>))
//             {
//                 context.Value = new Func<AppDbContext>(CreateDbContext);
//             }
//         }
//
//         public AppDbContext CreateDbContext()
//         {
//             var builder = new DbContextOptionsBuilder<AppDbContext>()
//                 .AddInterceptors(new TimestampInterceptor())
//                 .EnableDetailedErrors()
//                 .EnableSensitiveDataLogging()
//                 .UseSqlite(_sqliteConnection.Value);
//
//             var dbContext = new AppDbContext(builder.Options);
//
//             dbContext.Database.EnsureCreated();
//             return dbContext;
//         }
//
//         private void Dispose(bool disposing)
//         {
//             if (!_disposedValue)
//             {
//                 _disposedValue = true;
//             }
//         }
//
//         public void Dispose()
//         {
//             // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
//             Dispose(disposing: true);
//             // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
//             GC.SuppressFinalize(this);
//         }
//     }
// }
