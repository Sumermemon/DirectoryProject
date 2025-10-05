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
        public string? Gender { get; set; }
        public string? Qualification { get; set; }
        public int? RegNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string? MobileNo { get; set; }
        public bool IsActive { get; set; }
    }
}
