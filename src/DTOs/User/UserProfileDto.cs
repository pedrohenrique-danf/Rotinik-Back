using System.ComponentModel.DataAnnotations;

namespace Rotinik.DTOs.User;

public class UserProfileDto
{
    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}
