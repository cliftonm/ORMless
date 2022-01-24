using System;

using Interfaces;
using Models;

using Record = System.Collections.Generic.IDictionary<string, object>;

namespace Clifton.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAppDbContext context;

        public AuditService(IAppDbContext context)
        {
            this.context = context;
        }

        public void Insert(string entityName, int entityId, Record before, Record after, string action)
        {
            var audit = new Audit()
            {
                Entity = entityName,
                EntityId = entityId,
                RecordBefore = before.Serialize(),
                RecordAfter = after.Serialize(),
                Action = action,
                ActionBy = "",
                ActionDate = DateTime.Now,
            };

            // Use EF for this.
            context.Audit.Add(audit);
            context.SaveChanges();
        }
    }
}
