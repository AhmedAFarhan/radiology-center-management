using System.ComponentModel.DataAnnotations;

namespace RadiologyCenter.Desktop.Features.Identity.Models;

internal sealed class UserFormModel
{
    [Required(ErrorMessage = "validation.usernameRequired")]
    [MaxLength(256, ErrorMessage = "validation.usernameMaxLength")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.emailRequired")]
    [EmailAddress(ErrorMessage = "validation.emailInvalid")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.firstNameRequired")]
    [MaxLength(200, ErrorMessage = "validation.firstNameMaxLength")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.lastNameRequired")]
    [MaxLength(200, ErrorMessage = "validation.lastNameMaxLength")]
    public string LastName { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "validation.passwordRequired")]
    [MinLength(8, ErrorMessage = "validation.passwordMinLength")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{8,}$", ErrorMessage = "validation.passwordLettersDigits")]
    public string Password { get; set; } = string.Empty;

    public IReadOnlyCollection<string> SelectedRoleIds { get; set; } = new List<string>();
}

internal sealed class UserLockModel
{
    [Required(ErrorMessage = "validation.lockUntilDateRequired")]
    public DateTime? LockUntilDate { get; set; }

    [Required(ErrorMessage = "validation.lockUntilTimeRequired")]
    public TimeSpan? LockUntilTime { get; set; }
}

internal sealed class RoleFormModel
{
    [Required(ErrorMessage = "validation.roleNameRequired")]
    [MaxLength(256, ErrorMessage = "validation.roleNameMaxLength")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "validation.descriptionMaxLength")]
    public string? Description { get; set; }
}
