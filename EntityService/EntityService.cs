using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Dapper;

using Interfaces;
using Lib;

using Record = System.Collections.Generic.IDictionary<string, object>;
using Records = System.Collections.Generic.List<System.Collections.Generic.IDictionary<string, object>>;
using Parameters = System.Collections.Generic.Dictionary<string, object>;

namespace Clifton.Services
{
    public class EntityService : IEntityService
    {
        private readonly IDatabaseService dbSvc;
        private readonly IAuditService auditSvc;

        public EntityService(IDatabaseService dbSvc, IAuditService auditSvc)
        {
            this.dbSvc = dbSvc;
            this.auditSvc = auditSvc;
        }

        /// <summary>
        /// Returns the DapperRow collection as a collection of IDictionary string-object pairs.
        /// </summary>
        public Records GetAll(string tableName, Conditions where = null, Joins joins = null, bool hasDeleted = true)
        {
            var ret = Query(tableName, null, QueryFnc, where, joins, hasDeleted).ToList();

            return ret;
        }

        public List<T> GetAll<T>(string tableName, Conditions where = null, Joins joins = null, bool hasDeleted = true) where T : new()
        {
            var ret = Query(tableName, null, QueryFnc<T>, where, joins, hasDeleted).ToList();

            return ret;
        }

        /// <summary>
        /// Returns the DapperRow as IDictionary string-object pairs.
        /// </summary>
        public Record GetSingle(string tableName, int recordId, Joins joins = null)
        {
            var ret = Query(tableName, recordId, QueryFnc, null, joins).SingleOrDefault();

            return ret;
        }

        /// <summary>
        /// Returns the DapperRow as IDictionary string-object pairs.
        /// </summary>
        public Record GetSingle(string tableName, Conditions where)
        {
            var ret = Query(tableName, null, QueryFnc, where).SingleOrDefault();

            return ret;
        }

        /// <summary>
        /// Returns the DapperRow as IDictionary string-object pairs.
        /// </summary>
        public Record GetSingle(string tableName, Conditions where, Joins joins = null)
        {
            var ret = Query(tableName, null, QueryFnc, where, joins).SingleOrDefault();

            return ret;
        }

        public Record GetById(string tableName, int entityId)
        {
            var where = Conditions.Where().Field(Constants.ID).Is(entityId);
            var ret = Query(tableName, null, QueryFnc, where).SingleOrDefault();

            return ret;
        }

        public Record Insert(string tableName, Parameters parms)
        {
            // Returns the full record.
            var ret = Insert(tableName, parms, QueryFnc).SingleOrDefault();
            auditSvc.Insert(tableName, ret[Constants.ID].ToInt(), null, ret, Constants.AUDIT_INSERT);

            return ret;
        }

        public Record Update(string tableName, int entityId, Parameters parms)
        {
            var before = GetById(tableName, entityId);
            var ret = Update(tableName, entityId, parms, QueryFnc).SingleOrDefault();
            auditSvc.Insert(tableName, entityId, before, ret, Constants.AUDIT_UPDATE);

            return ret;
        }

        public void SoftDelete(string tableName, int entityId)
        {
            var before = GetById(tableName, entityId);
            var parms = new Parameters() { { "ID", entityId }, { Constants.DELETED, true } };
            Update(tableName, entityId, parms, QueryFnc, asDelete: true).SingleOrDefault();
            auditSvc.Insert(tableName, entityId, before, null, Constants.AUDIT_DELETE);
        }

        public void HardDelete(string tableName, int entityId)
        {
            var before = GetById(tableName, entityId);
            using var conn = dbSvc.GetSqlConnection();
            conn.Execute($"delete from {tableName} where {Constants.ID} = @id", new { id = entityId });
            auditSvc.Insert(tableName, entityId, before, null, Constants.AUDIT_DELETE);
        }

        private List<T> Query<T>(string tableName, int? id, Func<SqlConnection, (string sql, Parameters parms), IEnumerable<T>> query, Conditions where = null, Joins joins = null, bool hasDeleted = true)
        {
            using var conn = dbSvc.GetSqlConnection();
            var qinfo = SqlSelectBuilder(tableName, id, where, joins, hasDeleted);
            var ret = query(conn, qinfo).ToList();

            return ret;
        }

        private Records Insert(string tableName, Parameters parms, Func<SqlConnection, (string sql, Parameters parms), Records> query, Joins joins = null)
        {
            using var conn = dbSvc.GetSqlConnection();

            var qinfo = SqlInsertBuilder(tableName, parms, joins);
            var ret = query(conn, qinfo).ToList();

            return ret;
        }

        private Records QueryFnc(SqlConnection conn, (string sql, Parameters parms) qinfo)
        {
            try
            {
                var records = conn.Query(qinfo.sql, qinfo.parms).Cast<Record>().ToList();

                return records;
            }
            catch (Exception ex)
            {
                throw new Exception($"SQL Exception:{ex.Message}\r\n{qinfo.sql}");
            }
        }

        private List<T> QueryFnc<T>(SqlConnection conn, (string sql, Parameters parms) qinfo)
        {
            try
            {
                var records = conn.Query<T>(qinfo.sql, qinfo.parms).ToList();

                return records;
            }
            catch (Exception ex)
            {
                throw new Exception($"SQL Exception:{ex.Message}\r\n{qinfo.sql}");
            }
        }

        private Records Update(string tableName, int id, Parameters parms, Func<SqlConnection, (string sql, Parameters parms), Records> query, bool asDelete = false)
        {
            using var conn = dbSvc.GetSqlConnection();
            var qinfo = SqlUpdateBuilder(tableName, id, parms, asDelete);
            var ret = query(conn, qinfo).ToList();

            return ret;
        }

        private StringBuilder GetCoreSelect(string table, Joins joins = null, bool hasDeleted = true)
        {
            var joinFields = joins?.GetJoinFields(",") ?? "";
            var joinTables = joins?.GetJoins() ?? "";

            var withDeleteCheck = hasDeleted ? $"where {table}.{Constants.DELETED} = 0" : "";

            StringBuilder sb = new StringBuilder();
            sb.Append($"select {table}.* {joinFields} from {table} {joinTables} {withDeleteCheck}");

            return sb;
        }

        private (string sql, Parameters parms) SqlSelectBuilder(string table, int? id, Conditions where = null, Joins joins = null, bool hasDeleted = true)
        {
            var sb = GetCoreSelect(table, joins, hasDeleted);

            var parms = new Parameters();

            if (id != null)
            {
                sb.Append($" and {table}.{Constants.ID} = @{Constants.ID}");
                parms.Add(Constants.ID, id.Value);
            }

            where?.AddConditions(sb, parms);

            return (sb.ToString(), parms);
        }

        private (string sql, Parameters parms) SqlInsertSelectBuilder(string table, Conditions where = null, Joins joins = null)
        {
            var sb = GetCoreSelect(table, joins);
            sb.Append($" and {table}.{Constants.ID} in (SELECT CAST(SCOPE_IDENTITY() AS INT))");

            var parms = new Parameters();

            return (sb.ToString(), parms);
        }

        private (string sql, Parameters parms) SqlInsertBuilder(string table, Parameters parms, Joins joins = null)
        {
            if (parms.ContainsKey(Constants.ID))
            {
                parms.Remove(Constants.ID);
            }

            parms[Constants.DELETED] = false;
            var cols = String.Join(", ", parms.Keys.Select(k => k));
            var vals = String.Join(", ", parms.Keys.Select(k => $"@{k}"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"insert into {table} ({cols}) values ({vals});");
            var query = SqlInsertSelectBuilder(table, joins: joins).sql;
            sb.Append(query);

            return (sb.ToString(), parms);
        }

        private (string sql, Parameters parms) SqlUpdateBuilder(string table, int id, Parameters parms, bool asDelete = false)
        {
            // Remove any ID, Deleted field -- we don't update the deleted flag here.
            parms.Remove(Constants.ID);

            if (!asDelete)
            {
                parms.Remove(Constants.DELETED);
            }

            var setters = String.Join(", ", parms.Keys.Select(k => $"{k}=@{k}"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"update {table} set {setters} where {Constants.ID} = @{Constants.ID};");
            var query = SqlSelectBuilder(table, id).sql;
            sb.Append(query);

            // Add at end, as we're not setting the ID.  Here we are setting the parameter, so "id" is OK.
            parms[Constants.ID] = id;

            return (sb.ToString(), parms);
        }
    }
}
