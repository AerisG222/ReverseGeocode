using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql;


namespace ReverseGeocode.Data
{
    public abstract class Database
    {
        string _connString;


        public Database(string connString)
        {
            if(string.IsNullOrEmpty(connString))
            {
                throw new ArgumentNullException(nameof(connString));
            }

            _connString = connString;
        }


        Task<T> Query<T>(string sql)
        {

        }


        async Task<IDbConnection> GetConnection()
        {
            var conn = new NpgsqlConnection(_connString);

            await conn.OpenAsync();

            return conn;
        }
    }
}
