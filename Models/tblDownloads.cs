// Models/tblDownload.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml;

namespace DOAN.Models
{
    [Table("Downloads")]
    public class tblDownload
    {
        [Key]
        public int DownloadID { get; set; }

        public int DocumentID { get; set; }

        [ForeignKey("DocumentID")]
        public virtual tblDocuments? Document { get; set; }

        public int UserID { get; set; }

        public DateTime DownloadDate { get; set; }
    }
}
