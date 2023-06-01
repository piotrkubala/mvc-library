using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLibrary.Models;

public class BookCategoryModel {
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int BookCategoryId { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = "";
}