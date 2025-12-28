using System.Data;
using Npgsql;

namespace ReverseGeocode.Data;

public abstract class Database
{
    readonly string _connString;

    public Database(string connString)
    {
        if (string.IsNullOrEmpty(connString))
        {
            throw new ArgumentNullException(nameof(connString));
        }

        _connString = connString;
    }

    protected async Task<T> RunAsync<T>(Func<IDbConnection, Task<T>> queryData)
    {
        if (queryData == null)
        {
            throw new ArgumentNullException(nameof(queryData));
        }

        using (var conn = await GetConnectionAsync())
        {
            return await queryData(conn).ConfigureAwait(false);
        }
    }

    protected async Task RunAsync(Func<IDbConnection, Task> executeStatement)
    {
        if (executeStatement == null)
        {
            throw new ArgumentNullException(nameof(executeStatement));
        }

        using (var conn = await GetConnectionAsync())
        {
            await executeStatement(conn).ConfigureAwait(false);
        }
    }

    async Task<IDbConnection> GetConnectionAsync()
    {
        var conn = new NpgsqlConnection(_connString);

        await conn.OpenAsync().ConfigureAwait(false);

        return conn;
    }
}
