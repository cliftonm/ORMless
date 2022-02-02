using System.ComponentModel.DataAnnotations;

namespace IntegrationTests.Models
{
    public class Entity
    {
        [Key]
        public int ID { get; set; }
        public string TableName { get; set; }
        public bool Deleted { get; set; }
    }
}
