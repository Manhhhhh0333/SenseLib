using System.ComponentModel.DataAnnotations;

namespace DOAN.ViewModel
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }
    }
}
