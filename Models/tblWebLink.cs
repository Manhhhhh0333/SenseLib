using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOAN.Models
{
    [Table("WebLinks")]
    public class tblWebLink
    {
        [Key]
        public int WebLinkID { get; set; }
        public string? Name { get; set; }
        public string? URL { get; set; }
        public bool? Status { get; set; }
      
    }
}
