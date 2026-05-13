namespace RotinikApi.DTOs.Requests
{
    public class UserCreateRequest
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Password { get; set; }
    }
}