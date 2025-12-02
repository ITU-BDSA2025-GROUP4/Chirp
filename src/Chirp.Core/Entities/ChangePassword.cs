using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.Entities;

public class ChangePasswordForm {
    public string PreviousPassword { get; set; } = null!;

    public string NewPassword { get; set; } = null!;

    public string NewPasswordConfirm { get; set;} = null!;
}