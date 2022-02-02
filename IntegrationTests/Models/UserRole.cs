using System.ComponentModel.DataAnnotations;

namespace IntegrationTests.Models
{
    public class UserRole
    {
        [Key]
        public int ID { get; set; }
        public int RoleID { get; set; }
        public int UserID { get; set; }
        public bool Deleted { get; set; }
    }
}
