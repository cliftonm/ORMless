using System.ComponentModel.DataAnnotations;

namespace IntegrationTests.Models
{
    public class EntityRole
    {
        [Key]
        public int ID { get; set; }
        public int RoleID { get; set; }
        public int EntityID { get; set; }
        public bool Deleted { get; set; }
    }
}
