using System.ComponentModel.DataAnnotations;

namespace RadiologyCenter.Desktop.Features.Auth.Models;

internal sealed class LoginModel
{
    [Required(ErrorMessage = "validation.usernameRequired")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.passwordRequired")]
    public string Password { get; set; } = string.Empty;
}

internal sealed class ChangePasswordModel
{
    [Required(ErrorMessage = "validation.currentPasswordRequired")]
    public string CurrentPassword { get; set; } = string.Empty;

    [MinLength(8, ErrorMessage = "validation.passwordMinLength")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{8,}$", ErrorMessage = "validation.passwordLettersDigits")]
    public string NewPassword { get; set; } = string.Empty;

    [Compare(nameof(NewPassword), ErrorMessage = "validation.passwordMismatch")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

internal sealed class ResetPasswordModel
{
    [MinLength(8, ErrorMessage = "validation.passwordMinLength")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{8,}$", ErrorMessage = "validation.passwordLettersDigits")]
    public string NewPassword { get; set; } = string.Empty;

    [Compare(nameof(NewPassword), ErrorMessage = "validation.passwordMismatch")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
