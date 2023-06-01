using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLibrary.Models;

public class UserModel {
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int UserId { get; set; }

    [Required]
    [StringLength(50), MinLength(3)]
    public string Username { get; set; } = "";

    [Required]
    [StringLength(256), DataType(DataType.Password), MinLength(8)]
    public string PasswordHash { get; set; } = "";

    [Required]
    [StringLength(128)]
    public string PasswordSalt { get; set; } = "";

    [Required]
    [StringLength(256)]
    public string APIKey { get; set; } = "";

    [Required]
    public bool IsAdmin { get; set; } = false;

    [Required]
    public bool IsApproved { get; set; } = false;
}