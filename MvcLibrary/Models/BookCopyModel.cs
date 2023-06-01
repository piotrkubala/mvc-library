using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLibrary.Models;

public class BookCopyModel {
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int BookCopyId { get; set; }

    [Required]
    [ForeignKey("Book")]
    public int BookId { get; set; }
}