using System.ComponentModel.DataAnnotations;

namespace AutoSalonGrida.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите логин.")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
