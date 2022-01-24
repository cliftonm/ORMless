using System.Data.SqlClient;

using Microsoft.Extensions.Configuration;

using Interfaces;
using Lib;

namespace Clifton.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IConfiguration cfg;

        public DatabaseService(IConfiguration cfg)
        {
            this.cfg = cfg;
        }

        public SqlConnection GetSqlConnection()
        {
            var cs = cfg.GetConnectionString(AppSettings.Settings.UseDatabase);
            var conn = new SqlConnection(cs);

            return conn;
        }

        // TODO:
        // https://docs.microsoft.com/en-us/ef/core/saving/transactions
        // conn.BeginTransaction();

    }
}
