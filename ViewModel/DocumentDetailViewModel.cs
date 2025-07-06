using DOAN.Models;

namespace DOAN.ViewModel
{
    public class DocumentDetailViewModel
    {
        public tblDocuments Document { get; set; }

        public List<CommentWithUserViewModel> Comments { get; set; }

        public tblComments NewComment { get; set; } = new tblComments();

        // 🆕 Thêm để hiển thị tên loại tài liệu và nhà xuất bản
        public string? DocumentTypeName { get; set; }
        public string? PublisherName { get; set; }

        public double? AverageStar { get; set; }
    }

    public class CommentWithUserViewModel
    {
        public int CommentID { get; set; }
        public int DocumentID { get; set; }
        public string? FullName { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
