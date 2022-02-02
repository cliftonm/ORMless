using System.ComponentModel.DataAnnotations;

namespace IntegrationTests.Models
{
    public class Entity
    {
        [Key]
        public int Id { get; set; }
        public string TableName { get; set; }
        public bool Deleted { get; set; }
    }
}
