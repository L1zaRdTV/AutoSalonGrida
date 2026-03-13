using System.ComponentModel.DataAnnotations;

namespace AutoSalonGrida.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    [Display(Name = "Category")]
    public string Category { get; set; } = string.Empty;

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(50)]
    [Display(Name = "Status")]
    public string Status { get; set; } = "Active";

    [Range(0, 1000000)]
    [Display(Name = "Price")]
    public decimal Price { get; set; }
}
