namespace Rotinik.Features.Users.DTOs;

public class UserRegistrationDto
{
    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string Password { get; set; } = string.Empty;
}