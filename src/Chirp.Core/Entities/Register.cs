using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.Entities;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "UserName")]
    [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters.")]
    public string UserNamer { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; } = null!;

    // TODO: add use later
    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime? DateOfBirth { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
    public string Password { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Pawwrod and confirmation password do not match")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = null!;
}