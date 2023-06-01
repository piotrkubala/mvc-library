using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLibrary.Models;

public class BookModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BookId { get; set; }

    [Required]
    [StringLength(250)]
    public string Title { get; set; } = "";

    [Required]
    [StringLength(250)]
    public string Author { get; set; } = "";

    [Required]
    [StringLength(250)]
    public string Publisher { get; set; } = "";

    [Required]
    [StringLength(250)]
    public string Isbn { get; set; } = "";

    [Required]
    public string Description { get; set; } = "";

    [DataType(DataType.Date)]
    public DateTime? PublicationDate { get; set; }

    [Required]
    [ForeignKey("BookCategory")]
    public int BookCategoryId { get; set; }    
}