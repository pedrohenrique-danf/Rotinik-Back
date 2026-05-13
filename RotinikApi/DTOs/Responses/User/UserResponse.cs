namespace RotinikApi.DTOs.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Username { get; set; } // Adicionado
        public required string Email { get; set; }
        public required DateTime BirthDate { get; set; } // Adicionado
        public string? Phone { get; set; } // Transformado em opcional para evitar erro de inicialização
        public DateTime CreatedAt { get; set; }
    }
}