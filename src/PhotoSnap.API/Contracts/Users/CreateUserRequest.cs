using System.ComponentModel.DataAnnotations;

namespace PhotoSnap.API.Contracts.Users;

public sealed class CreateUserRequest
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(320)]
    public string Email { get; init; } = string.Empty;
}
