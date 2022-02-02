using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool IsSysAdmin { get; set; }
        public DateTime? LastLogin { get; set; }
        public int? ExpiresIn { get; set; }
        public long? ExpiresOn { get; set; }
        public bool Deleted { get; set; }

        public void Logout()
        {
            AccessToken = null;
            RefreshToken = null;
            ExpiresIn = null;
            ExpiresOn = null;
        }
    }
}
