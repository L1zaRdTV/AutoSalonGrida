using System.ComponentModel.DataAnnotations;

namespace AutoSalonGrida.Models
{
    public class Role
    {
        [Key]
        public int IdRole { get; set; }

        [Required(ErrorMessage = "Укажите название роли.")]
        [StringLength(50)]
        [Display(Name = "Название роли")]
        public string Name { get; set; } = string.Empty;

        public ICollection<User>? Users { get; set; }
    }
}
