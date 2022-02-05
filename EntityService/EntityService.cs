using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Dapper;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

using Models;
using Interfaces;
using Lib;

using Record = System.Collections.Generic.IDictionary<string, object>;
using Records = System.Collections.Generic.List<System.Collections.Generic.IDictionary<string, object>>;
using Parameters = System.Collections.Generic.Dictionary<string, object>;

namespace Clifton.Services
{
    public static class SqlKataExtensionMethods
    {
        public static Query Query<T>(this QueryFactory qf)
        {
            return qf.Query(typeof(T).Name);
        }

        public static Query Join<R, T>(this Query q)
        {
            var rname = typeof(R).Name;
            var tname = typeof(T).Name; 

            return q.Join($"{tname}", $"{rname}.Id", $"{tname}.{rname}Id");
        }

        public static Query JoinChild<R, T>(this Query q)
        {
            var rname = typeof(R).Name;
            var tname = typeof(T).Name;

            return q.Join($"{tname}", $"{tname}.Id", $"{rname}.{tname}Id");
        }

        public static Query Where<T>(this Query q, string field, object val)
        {
            return q.Where($"{typeof(T).Name}.{field}", val);
        }
    }

    public class Role { }
    public class UserRole 
    {
        public int UserId { get; set; }
    }
    public class EntityRole { }
    public class Entity 
    {
        public string TableName { get; set; }
    }

    public class EntityService : IEntityService
    {
        private readonly IDatabaseService dbSvc;
        private readonly IAuditService auditSvc;

        public EntityService(IDatabaseService dbSvc, IAuditService auditSvc)
        {
            this.dbSvc = dbSvc;
            this.auditSvc = auditSvc;
        }

        public bool IsEntityValid(string entityName)
        {
            // Get tables in the DB, not tables listed in the Entity table.
            // SELECT * FROM INFORMATION_SCHEMA.TABLES 

            var recs = GetAll("TABLES", Conditions.Where().Field("TABLE_NAME").Is(entityName), hasDeleted: false, schema: "INFORMATION_SCHEMA");

            return recs.Any();
        }

        public bool IsUserActionAuthorized(string entityName, int userId, string method)
        {
            var connection = dbSvc.GetSqlConnection();
            var compiler = new SqlServerCompiler();

            var db = new QueryFactory(connection, compiler);

            /*
            var query = db.Query("Role")
                .Join("UserRole", "Role.Id", "UserRole.RoleId")
                .Join("EntityRole", "Role.Id", "EntityRole.RoleId")
                .Join("Entity", "Entity.Id", "EntityRole.EntityId")
                .Where("Entity.TableName", entityName)
                .Where("UserRole.UserId", userId);
            */

            var query = db.Query<Role>()
                .Join<Role, UserRole>()
                .Join<Role, EntityRole>()
                .JoinChild<EntityRole, Entity>()
                .Where<Entity>(nameof(Entity.TableName), entityName)
                .Where<UserRole>(nameof(UserRole.UserId), userId);

            var data = query.Get<Permissions>();

            bool ok = method.MatchReturn(
                (m => m == "GET", _ => data.Any(d => d.CanRead)),
                (m => m == "POST", _ => data.Any(d => d.CanCreate)),
                (m => m == "PATCH", _ => data.Any(d => d.CanUpdate)),
                (m => m == "DELETE", _ => data.Any(d => d.CanDelete)));

            return ok;
        }

        /// <summary>
        /// Returns the DapperRow collection as a collection of IDictionary string-object pairs.
        /// </summary>
        public Records GetAll(string tableName, Conditions where = null, Joins joins = null, bool hasDeleted = true, string schema = null)
        {
            var ret = Query(tableName, null, QueryFnc, where, joins, hasDeleted, schema).ToList();

            return ret;
        }

        public List<T> GetAll<T>(string tableName, Conditions where = null, Joins joins = null, bool hasDeleted = true, string schema = null) where T : new()
        {
            var ret = Query(tableName, null, QueryFnc<T>, where, joins, hasDeleted, schema).ToList();

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
            conn.Execute($"delete from [{tableName}] where {Constants.ID} = @id", new { id = entityId });
            auditSvc.Insert(tableName, entityId, before, null, Constants.AUDIT_DELETE);
        }

        private List<T> Query<T>(string tableName, int? id, Func<SqlConnection, (string sql, Parameters parms), IEnumerable<T>> query, Conditions where = null, Joins joins = null, bool hasDeleted = true, string schema = null)
        {
            using var conn = dbSvc.GetSqlConnection();
            var qinfo = SqlSelectBuilder(tableName, id, where, joins, hasDeleted, schema);
            var ret = query(conn, qinfo).ToList();

            return ret;
        }

        private Records Insert(string tableName, Parameters parms, Func<SqlConnection, (string sql, Parameters parms), Records> query)
        {
            using var conn = dbSvc.GetSqlConnection();

            var qinfo = SqlInsertBuilder(tableName, parms);
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

        private StringBuilder GetCoreSelect(string table, Joins joins = null, bool hasDeleted = true, bool hasWhere = false, string schema = null)
        {
            var joinFields = joins?.GetJoinFields(",") ?? "";
            var joinTables = joins?.GetJoins() ?? "";
            var schemaStr = String.IsNullOrEmpty(schema) ? "" : $"[{schema}].";

            var withDeleteCheck = hasDeleted ? $"where [{table}].{Constants.DELETED} = 0" : "";

            StringBuilder sb = new StringBuilder();
            sb.Append($"select {schemaStr}[{table}].* {joinFields} from {schemaStr}[{table}] {joinTables} {withDeleteCheck}");

            if (!hasDeleted && hasWhere)
            {
                // eww!
                sb.Append(" where 1=1");
            }

            return sb;
        }

        private (string sql, Parameters parms) SqlSelectBuilder(string table, int? id, Conditions where = null, Joins joins = null, bool hasDeleted = true, string schema = null)
        {
            var sb = GetCoreSelect(table, joins, hasDeleted, where != null, schema);

            var parms = new Parameters();

            if (id != null)
            {
                sb.Append($" and [{table}].{Constants.ID} = @{Constants.ID}");
                parms.Add(Constants.ID, id.Value);
            }

            where?.AddConditions(sb, parms);

            return (sb.ToString(), parms);
        }

        private (string sql, Parameters parms) SqlInsertSelectBuilder(string table)
        {
            // On an insert, the record will of course exist.
            var sb = GetCoreSelect(table, hasDeleted: false);       
            sb.Append($" where [{table}].{Constants.ID} in (SELECT CAST(SCOPE_IDENTITY() AS INT))");

            var parms = new Parameters();

            return (sb.ToString(), parms);
        }

        private (string sql, Parameters parms) SqlInsertBuilder(string table, Parameters parms)
        {
            if (parms.ContainsKey(Constants.ID))
            {
                parms.Remove(Constants.ID);
            }

            parms[Constants.DELETED] = false;
            var cols = String.Join(", ", parms.Keys.Select(k => $"[{k}]"));
            var vals = String.Join(", ", parms.Keys.Select(k => $"@{k}"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"insert into [{table}] ({cols}) values ({vals});");
            var query = SqlInsertSelectBuilder(table).sql;
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

            var setters = String.Join(", ", parms.Keys.Select(k => $"[{k}]=@{k}"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"update [{table}] set {setters} where {Constants.ID} = @{Constants.ID};");
            var query = SqlSelectBuilder(table, id).sql;
            sb.Append(query);

            // Add at end, as we're not setting the ID.  Here we are setting the parameter, so "id" is OK.
            parms[Constants.ID] = id;

            return (sb.ToString(), parms);
        }
    }
}
