using System.ComponentModel.DataAnnotations;
public class RegisterDto
{
    public required string Username { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "password should be between 8 to 100 characters.")]
    public required string Password { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }
}