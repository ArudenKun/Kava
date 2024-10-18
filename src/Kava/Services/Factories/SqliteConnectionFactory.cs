using System.Data;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;
using Kava.Services.Abstractions.Factories;

namespace Kava.Services.Factories;

public class SqliteConnectionFactory : IDbConnectionFactory
{
    public SqliteConnectionFactory(string connectionString)
        : this(builder => builder.ConnectionString = connectionString) { }

    public SqliteConnectionFactory(Action<SQLiteConnectionStringBuilder> action)
    {
        var builder = new SQLiteConnectionStringBuilder();
        action(builder);
        ConnectionString = builder.ConnectionString;
    }

    public string ConnectionString { get; }

    public IDbConnection Create()
    {
        var conn = new SQLiteConnection(ConnectionString);
        conn.Open();
        return conn;
    }

    public async Task<IDbConnection> CreateAsync(CancellationToken cancellationToken = default)
    {
        var conn = new SQLiteConnection(ConnectionString);
        await conn.OpenAsync(cancellationToken);
        return conn;
    }
}
