using Record = System.Collections.Generic.IDictionary<string, object>;

namespace Interfaces
{
    public interface IAuditService
    {
        void Insert(string entityName, int entityId, Record before, Record after, string action);
    }
}
