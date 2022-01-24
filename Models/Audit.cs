using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Audit
    {
        [Key]
        public int ID { get; set; }
        public string Entity { get; set; }
        public int EntityId { get; set; }
        public string RecordBefore { get; set; }
        public string RecordAfter { get; set; }
        public string Action { get; set; }
        public string ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public bool Deleted { get; set; }
    }
}
