using System.Data.SqlClient;

namespace Interfaces
{
    public interface IDatabaseService
    {
        SqlConnection GetSqlConnection();
    }
}
