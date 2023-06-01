using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLibrary.Models;

public class ReaderModel {
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int ReaderId { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = "";

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = "";
}