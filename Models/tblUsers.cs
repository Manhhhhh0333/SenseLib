using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOAN.Models
{
    [Table("Users")]
    public class tblUsers
    {
        [Key]
        public int UserID { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UserPoint { get; set; }
        public int? FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockoutEndTime { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool? IsLocked => LockoutEndTime.HasValue && LockoutEndTime > DateTime.Now;

        
    }
}
