using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLibrary.Models;

public class BorrowingModel {
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int BorrowingId { get; set; }

    [Required]
    [ForeignKey("BookCopy")]
    public int BookCopyId { get; set; }

    [Required]
    [ForeignKey("Reader")]
    public int ReaderId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime BorrowDate { get; set; } = DateTime.Now;

    [DataType(DataType.Date)]
    public DateTime ?ReturnDate { get; set; }
}