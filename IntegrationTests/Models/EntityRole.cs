using System.ComponentModel.DataAnnotations;

namespace IntegrationTests.Models
{
    public class EntityRole
    {
        [Key]
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int EntityId { get; set; }
        public bool Deleted { get; set; }
    }
}
