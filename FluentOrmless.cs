using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;

namespace ORMless
{
    public abstract class FluentOrmlessCommand
    {
        protected FluentOrmlessConnection conn;
        protected SqlCommand cmd;

        public string Sql { get { return cmd.CommandText; } }

        public FluentOrmlessCommand(FluentOrmlessConnection conn, SqlCommand cmd)
        {
            this.conn = conn;
            this.cmd = cmd;
        }
    }

    public class FluentOrmlessInsertCommand : FluentOrmlessCommand
    {
        private object data;

        public FluentOrmlessInsertCommand(FluentOrmlessConnection conn, SqlCommand cmd, object data) : base(conn, cmd)
        {
            this.cmd = cmd;
            this.data = data;
        }

        public dynamic Execute()
        {
            cmd.Connection.Open();
            // Interesting -- the return type is object(decimal)
            decimal id = Convert.ToDecimal(cmd.ExecuteScalar());
            cmd.Connection.Close();

            ExpandoObject ret = new ExpandoObject();
            data.GetType().GetProperties().ForEach(d => ((IDictionary<string, object>)ret)[d.Name] = d.GetValue(data));
            ((IDictionary<string, object>)ret)[conn.PkName] = id;

            return ret;
        }
    }

    public class FluentOrmlessUpdateCommand : FluentOrmlessCommand
    {
        private object data;

        public FluentOrmlessUpdateCommand(FluentOrmlessConnection conn, SqlCommand cmd, object data) : base(conn, cmd)
        {
            this.cmd = cmd;
            this.data = data;
        }

        public void Execute()
        {
            cmd.Connection.Open();
            decimal id = Convert.ToDecimal(cmd.ExecuteNonQuery());
            cmd.Connection.Close();
        }
    }

    public class FluentOrmlessDeleteCommand : FluentOrmlessCommand
    {
        private object data;

        public FluentOrmlessDeleteCommand(FluentOrmlessConnection conn, SqlCommand cmd, object data) : base(conn, cmd)
        {
            this.cmd = cmd;
            this.data = data;
        }

        public void Execute()
        {
            cmd.Connection.Open();
            decimal id = Convert.ToDecimal(cmd.ExecuteNonQuery());
            cmd.Connection.Close();
        }
    }

    public class FluentOrmlessSelectCommand : FluentOrmlessCommand
    {
        public FluentOrmlessSelectCommand(FluentOrmlessConnection conn, SqlCommand cmd) : base(conn, cmd)
        {
            this.cmd = cmd;
        }

        public DataTable Fill()
        {
            cmd.Connection.Open();
            DataTable dt = new DataTable();
            var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            cmd.Connection.Close();

            return dt;
        }
    }

    public class FluentOrmlessConnection
    {
        public string PkName { get; set; } = "Id";

        private SqlConnection conn;
        private string tableName;

        public FluentOrmlessConnection(string connStr)
        {
            conn = new SqlConnection(connStr);
        }

        public FluentOrmlessConnection ForTable(string tableName)
        {
            this.tableName = tableName;

            return this;
        }

        public FluentOrmlessSelectCommand Select(object template)
        {
            var fields = String.Join(",", template.GetType().GetProperties().Select(ct => ct.Name));
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"select {fields} from {tableName}";

            return new FluentOrmlessSelectCommand(this, cmd);
        }

        public FluentOrmlessInsertCommand Insert(object data)
        {
            var fields = String.Join(",", data.GetType().GetProperties().Select(ct => ct.Name));
            var atFields = String.Join(",", data.GetType().GetProperties().Select(ct => "@" + ct.Name));
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"insert into {tableName} ({fields}) values ({atFields}); SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddRange(data.GetType().GetProperties().Select(ct => new SqlParameter("@" + ct.Name, ct.GetValue(data))).ToArray());

            return new FluentOrmlessInsertCommand(this, cmd, data);
        }

        public FluentOrmlessUpdateCommand Update(object data)
        {
            var fields = String.Join(",", data.GetType().GetProperties().Where(ct => ct.Name != PkName).Select(ct => $"{ct.Name} = @{ct.Name}"));
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"update {tableName} set {fields} where {PkName} = @{PkName}";
            cmd.Parameters.AddRange(data.GetType().GetProperties().Select(ct => new SqlParameter("@" + ct.Name, ct.GetValue(data))).ToArray());

            return new FluentOrmlessUpdateCommand(this, cmd, data);
        }

        public FluentOrmlessDeleteCommand Delete(object data)
        {
            var fields = String.Join(" and ", data.GetType().GetProperties().Select(ct => $"{ct.Name} = @{ct.Name}"));
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"delete from {tableName} where {fields}";
            cmd.Parameters.AddRange(data.GetType().GetProperties().Select(ct => new SqlParameter("@" + ct.Name, ct.GetValue(data))).ToArray());

            return new FluentOrmlessDeleteCommand(this, cmd, data);
        }
    }

    public class FluentOrmless
    {
        public static FluentOrmlessConnection ConnectWith(string connStr)
        {
            return new FluentOrmlessConnection(connStr);
        }
    }
}
