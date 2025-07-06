using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOAN.Models
{
    [Table("Transactions")]
    public class tblTransactions
    {
        [Key]
        public int TransactionID { get; set; }
        public int UserID { get; set; }
        public string? Action { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int PointsChanged { get; set; }

        [ForeignKey("UserID")]
        public tblUsers? User { get; set; }
    }
}
