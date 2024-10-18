using System.Data;

namespace Kava.Data.DbUp;

/// <summary>
/// A database connection wrapper to manage underlying connection as a shared connection
/// during database upgrade.
/// <remarks>
/// if underlying connection is already opened then it will be kept as opened and will not be closed
/// otherwise it will be opened when object is created and closed when object is disposed
/// however it will not be disposed
/// </remarks>
/// </summary>
public class SharedConnection : IDbConnection
{
    private readonly bool _connectionAlreadyOpened;
    private readonly IDbConnection _connection;

    /// <summary>
    /// Constructs a new instance
    /// </summary>
    public SharedConnection(IDbConnection dbConnection)
    {
        _connection =
            dbConnection
            ?? throw new ArgumentNullException(nameof(dbConnection), "database connection is null");

        if (_connection.State == ConnectionState.Open)
            _connectionAlreadyOpened = true;
        else
            _connection.Open();
    }

    public IDbTransaction BeginTransaction(IsolationLevel il) => _connection.BeginTransaction(il);

    public IDbTransaction BeginTransaction() => _connection.BeginTransaction();

    public void ChangeDatabase(string databaseName) => _connection.ChangeDatabase(databaseName);

    public void Close() { } // shared underlying connection is not closed

    public string ConnectionString
    {
        get => _connection.ConnectionString;
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        set => _connection.ConnectionString = value;
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
    }

    public int ConnectionTimeout => _connection.ConnectionTimeout;

    public IDbCommand CreateCommand() => _connection.CreateCommand();

    public string Database => _connection.Database;

    public void Open()
    {
        if (_connection.State == ConnectionState.Closed)
            _connection.Open();
    }

    public ConnectionState State => _connection.State;

    public void Dispose() { } // shared underlying connection is not disposed

    public void DoClose()
    {
        // if shared underlying connection is opened by this object
        // it will be closed here, otherwise the connection is not closed
        if (!_connectionAlreadyOpened && _connection.State == ConnectionState.Open)
        {
            _connection.Close();
        }
    }
}
