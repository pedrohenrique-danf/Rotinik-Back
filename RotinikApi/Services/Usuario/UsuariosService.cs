#nullable enable
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RotinikApi.Data;
using RotinikApi.DTOs.Requests;
using RotinikApi.DTOs.Requests.Auth;
using RotinikApi.DTOs.Responses;
using RotinikApi.DTOs.Responses.Auth;
using  RotinikApi.Models;

namespace RotinikApi.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly RotinikContext _context;

        public UsuarioService(RotinikContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UsuarioResponse>> ListarAsync()
        {
            return await _context.Usuarios
                .Select(u => new UsuarioResponse
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    Telefone = u.Telefone,
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
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
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
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                DataCadastro = usuario.DataCadastro
            };
        }

        private static string GerarHash(string senha)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
            return Convert.ToHexString(bytes).ToLower();
        }
        
        public async Task<AuthResponse?> AutenticarAsync(AuthRequest dto)
        {
            // 1. Gere o hash da senha que o usuário acabou de digitar
            var senhaDigitadaHash = GerarHash(dto.Senha);

            // 2. Agora compare HASH com HASH no banco
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Senha == senhaDigitadaHash);

            if (usuario == null) return null;

            return new AuthResponse
            {
                Token = "token_temporario_rotinik", 
                Usuario = new UsuarioResponse
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Telefone = usuario.Telefone,
                    DataCadastro = usuario.DataCadastro
                }
            };
        }
    }
}
