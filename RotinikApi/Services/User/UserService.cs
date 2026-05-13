#nullable enable
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RotinikApi.Data;
using RotinikApi.DTOs.Requests;
using RotinikApi.DTOs.Requests.Auth;
using RotinikApi.DTOs.Responses;
using RotinikApi.DTOs.Responses.Auth;
using RotinikApi.Models;
// Resolve ambiguity between System.Threading.Tasks.Task and RotinikApi.Services.Task namespace
using Task = System.Threading.Tasks.Task;

namespace RotinikApi.Services
{
    public class UserService : IUserService
    {
        private readonly RotinikContext _context;
        private readonly IConfiguration _config;

        public UserService(RotinikContext context, IConfiguration config)
        {
            _context = context;
            _config  = config;
        }

        public async Task<IEnumerable<UserResponse>> GetAllAsync()
        {
            return await _context.Users
                .Select(u => new UserResponse
                {
                    Id           = u.Id,
                    Name         = u.Name,
                    Username     = u.Username, // Mapeado
                    Email        = u.Email,
                    BirthDate    = u.BirthDate, // Corrigido erro de digitação
                    CreatedAt    = u.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<UserResponse> CreateAsync(UserCreateRequest dto)
        {
            // Validação de E-mail Único
            var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
                throw new Exception("A user with this email is already registered.");

            // Validação de Username Único (Crucial para o Rotinik)
            var usernameExists = await _context.Users.AnyAsync(u => u.Username == dto.Username);
            if (usernameExists)
                throw new Exception("This username is already taken.");

            // Criando a entidade com o novo construtor sincronizado com a tela
            var user = new User(
                dto.Name,
                dto.Username,    // Agora a Model aceita isso
                dto.BirthDate,   // Agora a Model aceita DateTime aqui
                dto.Email,
                GenerateHash(dto.Password)
            );

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapToResponse(user);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null)
                throw new Exception("User not found.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<UserResponse> LoginAsync(AuthRequest dto)
        {
            var passwordHash = GenerateHash(dto.Password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Password == passwordHash);

            if (user is null)
                throw new Exception("Invalid email or password.");

            return MapToResponse(user);
        }

        public async Task<AuthResponse?> AuthenticateAsync(AuthRequest dto)
        {
            var passwordHash = GenerateHash(dto.Password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Password == passwordHash);

            if (user == null) return null;

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                User = MapToResponse(user)
            };
        }

        public async Task<UserResponse?> GetMeAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null) return null;

            return MapToResponse(user);
        }

        // --- Helpers ---

        private static UserResponse MapToResponse(User u) => new()
        {
            Id        = u.Id,
            Name      = u.Name,
            Username  = u.Username,
            Email     = u.Email,
            BirthDate = u.BirthDate,
            CreatedAt = u.CreatedAt
        };

        private static string GenerateHash(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds       = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires     = DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpiresInHours"] ?? "8"));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username), // Adicionado Username no Token
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer:             jwtSettings["Issuer"],
                audience:           jwtSettings["Audience"],
                claims:             claims,
                expires:            expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}