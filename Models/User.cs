using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoSalonGrida.Models
{
    public class User
    {
        [Key]
        public int IdUser { get; set; }

        [Required(ErrorMessage = "Укажите имя.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Имя должно содержать от 2 до 100 символов.")]
        public string Name { get; set; } = string.Empty;

        [Range(18, 120, ErrorMessage = "Возраст должен быть в диапазоне от 18 до 120 лет.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Укажите логин.")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Логин должен содержать от 4 до 50 символов.")]
        public string Login { get; set; } = string.Empty;

        [StringLength(200)]
        public string PasswordHash { get; set; } = string.Empty;

        [Display(Name = "Роль")]
        public int IdRole { get; set; }

        [ForeignKey(nameof(IdRole))]
        public Role? Role { get; set; }
    }
}
