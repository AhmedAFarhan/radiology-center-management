using System.ComponentModel.DataAnnotations;

namespace RadiologyCenter.Desktop.Features.Identity.Models;

internal sealed class UserFormModel
{
    [Required(ErrorMessage = "Username is required.")]
    [MaxLength(256, ErrorMessage = "Username must be 256 characters or fewer.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(200, ErrorMessage = "First name must be 200 characters or fewer.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(200, ErrorMessage = "Last name must be 200 characters or fewer.")]
    public string LastName { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{8,}$", ErrorMessage = "Password must contain both letters and digits.")]
    public string Password { get; set; } = string.Empty;

    public IReadOnlyCollection<string> SelectedRoleIds { get; set; } = new List<string>();
}

internal sealed class UserLockModel
{
    [Required(ErrorMessage = "Lock until date is required.")]
    public DateTime? LockUntilDate { get; set; }

    [Required(ErrorMessage = "Lock until time is required.")]
    public TimeSpan? LockUntilTime { get; set; }
}

internal sealed class RoleFormModel
{
    [Required(ErrorMessage = "Role name is required.")]
    [MaxLength(256, ErrorMessage = "Role name must be 256 characters or fewer.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Description must be 500 characters or fewer.")]
    public string? Description { get; set; }
}
