using System.Data.SqlClient;

using Dapper;

namespace IntegrationTests
{
    public class Setup
    {
        protected string URL = "http://localhost/demo";
        private string connectionString = "Server=localhost;Database=DMS;Integrated Security=True;";

        public void ClearAllTables()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Execute("delete from Test");
                conn.Execute("delete from Audit");
                // Delete from the bottom of the FK references up to the parents.
            }
        }
    }
}
