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

namespace RotinikApi.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly RotinikContext _context;
        private readonly IConfiguration _config;

        public UsuarioService(RotinikContext context, IConfiguration config)
        {
            _context = context;
            _config  = config;
        }

        public async Task<IEnumerable<UsuarioResponse>> ListarAsync()
        {
            return await _context.Usuarios
                .Select(u => new UsuarioResponse
                {
                    Id           = u.Id,
                    Nome         = u.Nome,
                    Email        = u.Email,
                    Telefone     = u.Telefone,
                    DataCadastro = u.DataCadastro
                })
                .ToListAsync();
        }

        public async Task<UsuarioResponse> CriarAsync(UsuarioCriarRequest dto)
        {
            var emailExistente = await _context.Usuarios
                .AnyAsync(u => u.Email == dto.Email);

            if (emailExistente)
                throw new Exception("Já existe um usuário cadastrado com esse e-mail.");

            var usuario = new Usuario(
                dto.Nome,
                dto.Email,
                dto.Telefone,
                GerarHash(dto.Senha)
            );

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return new UsuarioResponse
            {
                Id           = usuario.Id,
                Nome         = usuario.Nome,
                Email        = usuario.Email,
                Telefone     = usuario.Telefone,
                DataCadastro = usuario.DataCadastro
            };
        }

        public async Task RemoverAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario is null)
                throw new Exception("Usuário não encontrado.");

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<UsuarioResponse> LoginAsync(AuthRequest dto)
        {
            var senhaHash = GerarHash(dto.Senha);

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Senha == senhaHash);

            if (usuario is null)
                throw new Exception("E-mail ou senha inválidos.");

            return new UsuarioResponse
            {
                Id           = usuario.Id,
                Nome         = usuario.Nome,
                Email        = usuario.Email,
                Telefone     = usuario.Telefone,
                DataCadastro = usuario.DataCadastro
            };
        }

        public async Task<AuthResponse?> AutenticarAsync(AuthRequest dto)
        {
            var senhaDigitadaHash = GerarHash(dto.Senha);

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Senha == senhaDigitadaHash);

            if (usuario == null) return null;

            var token = GerarTokenJwt(usuario);

            return new AuthResponse
            {
                Token = token,
                Usuario = new UsuarioResponse
                {
                    Id           = usuario.Id,
                    Nome         = usuario.Nome,
                    Email        = usuario.Email,
                    Telefone     = usuario.Telefone,
                    DataCadastro = usuario.DataCadastro
                }
            };
        }

        public async Task<UsuarioResponse?> MeAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (usuario is null) return null;

            return new UsuarioResponse
            {
                Id           = usuario.Id,
                Nome         = usuario.Nome,
                Email        = usuario.Email,
                Telefone     = usuario.Telefone,
                DataCadastro = usuario.DataCadastro
            };
        }

        // --- Helpers ---

        private static string GerarHash(string senha)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
            return Convert.ToHexString(bytes).ToLower();
        }

        private string GerarTokenJwt(Usuario usuario)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds       = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires     = DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpiresInHours"] ?? "8"));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Name,  usuario.Nome),
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
