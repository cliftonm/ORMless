using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class VersionInfo
    {
        [Key]
        public long Version { get; set; }
        public DateTime AppliedOn { get; set; }
        public string Description { get; set; }
    }
}
