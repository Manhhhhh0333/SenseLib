using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml;

namespace DOAN.Models
{
    [Table("Favorites")]
    public class tblFavorites
    {
        [Key]
        public int UserID { get; set; }
        public int DocumentID { get; set; }
        public DateTime? CreatedAt { get; set; }

        [ForeignKey("DocumentID")]
        public virtual tblDocuments? Document { get; set; }

        [ForeignKey("UserID")]
        public virtual tblUsers? User { get; set; }
    }
}
