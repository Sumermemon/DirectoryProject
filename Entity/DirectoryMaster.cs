using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DirectoryProject.Entity
{

    [Table("DirectoryMaster")]
    public class DirectoryMaster
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Qualification { get; set; }
        public string? GTMDANo { get; set; }
        public string Address { get; set; }
        public string? MobileNo { get; set; }
        public bool IsActive { get; set; }
    }
}
