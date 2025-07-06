using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOAN.Models
{
    [Table("Documents")]
    public class tblDocuments
    {
        [Key]
        public int DocumentID { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FileType { get; set; }
        public string? FilePath { get; set; }
        public string? Status { get; set; }
        public int CategoryID { get; set; }
        public int PublisherID { get; set; }
        public int AuthorID { get; set; }

        public int? UploadedBy { get; set; } // ID của người đăng
        public string? Image { get; set; }
        public int? DocumentPoint { get; set; }
        public DateTime? UploadDate { get; set; }

        [ForeignKey("AuthorID")]
        public tblAuthors? Author { get; set; }

        [ForeignKey("UploadedBy")]
        public tblUsers? UploadedUser { get; set; } // ✅ Navigation tới người dùng đăng tài liệu
    }
}
