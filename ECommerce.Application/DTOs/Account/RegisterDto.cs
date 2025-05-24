// ECommerce.Application/DTOs/Account/RegisterDto.cs
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Account
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "{0} en az {2} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Şifre ve şifre tekrarı eşleşmiyor.")]
        public string ConfirmPassword { get; set; }
    }
}