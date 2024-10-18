using System.Data.SQLite;
using System.Linq;
using System.Text.RegularExpressions;
using DbUp.Engine.Transactions;

namespace Kava.Data.DbUp;

/// <summary>
/// Connection manager for Sql Lite
/// </summary>
// ReSharper disable once InconsistentNaming
public class SQLiteConnectionManager : DatabaseConnectionManager
{
    /// <summary>
    /// Creates new SQLite Connection Manager
    /// </summary>
    public SQLiteConnectionManager(string connectionString)
        : base(l => new SQLiteConnection(connectionString)) { }

    /// <summary>
    /// Creates new SQLite Connection Manager
    /// </summary>
    public SQLiteConnectionManager(SharedConnection sharedConnection)
        : base(l => sharedConnection) { }

    /// <summary>
    /// Sqlite statements separator is ; (see http://www.sqlite.org/lang.html)
    /// </summary>
    public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
    {
        var scriptStatements = Regex
            .Split(scriptContents, "^\\s*;\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)
            .Select(x => x.Trim())
            .Where(x => x.Length > 0)
            .ToArray();

        return scriptStatements;
    }
}
