using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.Entities;

public class ResendConfirmationEmailViewModel
{
    [Required(ErrorMessage = "Email is Required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; } = null!;
}