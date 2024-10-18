using System.Data;
using Dapper;

namespace Kava.Data.Converters;

public class StringUlidHandler : SqlMapper.TypeHandler<Ulid>
{
    public override Ulid Parse(object value)
    {
        return Ulid.Parse((string)value);
    }

    public override void SetValue(IDbDataParameter parameter, Ulid value)
    {
        parameter.DbType = DbType.StringFixedLength;
        parameter.Size = 26;
        parameter.Value = value.ToString();
    }
}
