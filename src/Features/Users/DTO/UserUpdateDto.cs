namespace Rotinik.Features.Users.DTOs;

public class UserUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Password { get; set; } = string.Empty;
}
