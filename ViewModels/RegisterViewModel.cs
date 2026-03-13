using System.ComponentModel.DataAnnotations;

namespace AutoSalonGrida.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите имя.")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите возраст.")]
        [Range(18, 120)]
        public int Age { get; set; }

        [Required(ErrorMessage = "Введите логин.")]
        [StringLength(50, MinimumLength = 4)]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль.")]
        [StringLength(64, MinimumLength = 6, ErrorMessage = "Пароль должен содержать от 6 до 64 символов.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Подтвердите пароль.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
