using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOAN.Models
{
    [Table("Publishers")]
    public class tblPublishers
    {
        [Key]
        public int PublisherID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
