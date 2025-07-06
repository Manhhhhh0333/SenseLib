using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOAN.Models
{
    [Table("Authors")]
    public class tblAuthors
    {
        [Key]
        public int AuthorID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        // Nếu muốn ngược lại lấy danh sách tài liệu của tác giả:
        public ICollection<tblDocuments>? Documents { get; set; }
    }
}
