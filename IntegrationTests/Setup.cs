using System;
using System.Data.SqlClient;

using Dapper;

using IntegrationTests.Models;

namespace IntegrationTests
{
    public class Setup
    {
        protected string URL = "http://localhost/demo";
        private string connectionString = "Server=localhost;Database=DMS;Integrated Security=True;";

        public static Test testData = new Test()
        {
            IntField = 1,
            StringField = "test",
            DateField = DateTime.Parse("8/19/1962"),
            DateTimeField = DateTime.Parse("3/21/1991 7:47 pm"),
            TimeField = DateTime.Parse("12:05 am"),
            BitField = true
        };

        public static Test testData2 = new Test()
        {
            IntField = 2,
            StringField = "test2",
            DateField = DateTime.Parse("8/20/1962"),
            DateTimeField = DateTime.Parse("3/22/1991 7:47 pm"),
            TimeField = DateTime.Parse("12:06 am"),
            BitField = true
        };

        public void ClearAllTables()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Execute("delete from Test");
                conn.Execute("delete from Audit");

                // Delete from the bottom of the FK references up to the parents.

                conn.Execute("delete from UserRole");
                conn.Execute("delete from EntityRole");
                conn.Execute("delete from [User] where IsSysAdmin = 0");
                conn.Execute("delete from Entity");
            }
        }
    }
}
