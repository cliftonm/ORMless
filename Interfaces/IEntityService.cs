using System.Collections.Generic;

using Record = System.Collections.Generic.IDictionary<string, object>;
using Records = System.Collections.Generic.List<System.Collections.Generic.IDictionary<string, object>>;
using Parameters = System.Collections.Generic.Dictionary<string, object>;

using Lib;

namespace Interfaces
{
    public interface IEntityService
    {
        Records GetAll(string tableName, Conditions where = null, Joins joins = null, bool hasDeleted = true);
        List<T> GetAll<T>(string tableName, Conditions where = null, Joins joins = null, bool hasDeleted = true) where T : new();
        Record GetSingle(string tableName, int recordId, Joins joins = null);
        Record GetSingle(string tableName, Conditions where);
        Record GetSingle(string tableName, Conditions where, Joins joins = null);
        Record GetById(string tableName, int entityId);
        Record Insert(string tableName, Parameters parms);
        Record Update(string tableName, int entityId, Parameters parms);
        void SoftDelete(string tableName, int entityId);
        void HardDelete(string tableName, int entityId);
    }
}
