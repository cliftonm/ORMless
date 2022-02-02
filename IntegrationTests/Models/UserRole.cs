using System.ComponentModel.DataAnnotations;

namespace IntegrationTests.Models
{
    public class UserRole
    {
        [Key]
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public bool Deleted { get; set; }
    }
}
