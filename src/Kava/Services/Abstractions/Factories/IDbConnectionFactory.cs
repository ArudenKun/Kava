using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Kava.Services.Abstractions.Factories;

public interface IDbConnectionFactory : IFactory<IDbConnection>
{
    string ConnectionString { get; }
    Task<IDbConnection> CreateAsync(CancellationToken cancellationToken = default);
}
