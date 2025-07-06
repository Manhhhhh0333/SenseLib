using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOAN.Models
{
    [Table("OrderInfor")]
    public class MomoOrderInfo
    {
        [Key]
        public int Id { get; set; }
        public string? OrderInfo { get; set; }
        public string? FullName { get; set; }
        public Decimal Amount { get; set; }
        public DateTime DatePaid { get; set; }
        public string? OrderId { get; set; }
        public string? UserID { get; set; }
    }
}
