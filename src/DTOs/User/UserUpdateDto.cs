using System.ComponentModel.DataAnnotations;

namespace Rotinik.DTOs.User;

public class UserUpdateDto
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }
    
    public string Password { get; set; } = string.Empty;
}
