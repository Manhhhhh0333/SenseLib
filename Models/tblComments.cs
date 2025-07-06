using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml;

namespace DOAN.Models
{
    [Table("Comments")]
    public class tblComments
    {
        [Key]
        public int CommentID { get; set; }
        public int UserID { get; set; }
        public int DocumentID { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }

        [ForeignKey("DocumentID")]
        public virtual tblDocuments? Document { get; set; }

        [ForeignKey("UserID")]
        public virtual tblUsers? User { get; set; }

    }
}
