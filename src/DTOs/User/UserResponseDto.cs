using System.ComponentModel.DataAnnotations;

namespace Rotinik.DTOs.User;

public class UserResponseDto
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    public string Email { get; set; } = string.Empty;
}
