using DbUp.Support;

namespace Kava.Data.DbUp;

/// <summary>
/// Parses Sql Objects and performs quoting functions.
/// </summary>
// ReSharper disable once InconsistentNaming
public class SQLiteObjectParser : SqlObjectParser
{
    public SQLiteObjectParser()
        : base("[", "]") { }
}
