using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Extensions.Configuration;

using Dapper;
using FluentMigrator.Runner;

using Interfaces;
using Lib;

namespace Clifton.Services
{
    // Basic docs on how to this are here:
    // https://fluentmigrator.github.io/articles/quickstart.html?tabs=runner-in-process
    public class MigratorService : IMigratorService
    {
        private IMigrationRunner runner;
        private IConfiguration cfg;

        public MigratorService(IMigrationRunner runner, IConfiguration cfg)
        {
            this.runner = runner;
            this.cfg = cfg;
        }

        public string MigrateUp()
        {
            EnsureDatabase();
            var saved = Console.Out;
            var sb = new StringBuilder();
            var tw = new StringWriter(sb);
            Console.SetOut(tw);

            runner.MigrateUp();
            tw.Close();

            // Restore the default console out.
            // Simpler: https://stackoverflow.com/a/26095640
            Console.SetOut(saved);

            var errs = sb.ToString();
            var result = String.IsNullOrEmpty(errs) ? "Success" : errs;

            return result;
        }

        public void EnsureDatabase()
        {
            var cs = cfg.GetConnectionString(AppSettings.Settings.UseDatabase);
            var dbName = cs.RightOf("Database=").LeftOf(";");
            var master = cfg.GetConnectionString("MasterConnection");

            var parameters = new DynamicParameters();
            parameters.Add("name", dbName);
            using var connection = new SqlConnection(master);
            var records = connection.Query("SELECT name FROM sys.databases WHERE name = @name", parameters);

            if (!records.Any())
            {
                connection.Execute($"CREATE DATABASE [{dbName}]");
            }
        }
    }
}
