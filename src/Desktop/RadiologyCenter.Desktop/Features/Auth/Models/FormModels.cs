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

    [MinLength(12, ErrorMessage = "validation.passwordMinLength")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{12,128}$", ErrorMessage = "validation.passwordLettersDigits")]
    public string NewPassword { get; set; } = string.Empty;

    [Compare(nameof(NewPassword), ErrorMessage = "validation.passwordMismatch")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

internal sealed class ResetPasswordModel
{
    [MinLength(12, ErrorMessage = "validation.passwordMinLength")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{12,128}$", ErrorMessage = "validation.passwordLettersDigits")]
    public string NewPassword { get; set; } = string.Empty;

    [Compare(nameof(NewPassword), ErrorMessage = "validation.passwordMismatch")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
