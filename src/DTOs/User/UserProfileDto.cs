using System.ComponentModel.DataAnnotations;

namespace Rotinik_Backend.DTOs.User;

public class UserProfileDto
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;
}
