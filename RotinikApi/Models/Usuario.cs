using System;
using System.Text.RegularExpressions;

namespace RotinikApi.Models
{
    public class Usuario
    {
        public int Id { get; protected set; }
        public string Nome { get; protected set; }
        public string Email { get; protected set; }
        public string Telefone { get; protected set; }
        public string Senha { get; protected set; }
        public DateTime DataCadastro { get; protected set; }

        public Usuario(string nome, string email, string telefone, string senha)
        {
            SetNome(nome);
            SetEmail(email);
            SetTelefone(telefone);
            SetSenha(senha);
            SetDataCadastro(DateTime.UtcNow);
        }

        public void SetNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new Exception("O nome não pode ser vazio ou composto por espaços em branco");

            if (nome.Length < 3 || nome.Length > 150)
                throw new Exception("O nome deve ter entre 3 e 150 caracteres.");

            Nome = nome;
        }

        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("O e-mail não pode ser vazio ou composto por espaços em branco.");

            if (email.Length < 6 || email.Length > 150)
                throw new Exception("O e-mail deve ter entre 6 e 150 caracteres.");

            if (!Regex.IsMatch(email.Trim(), @"^[\w\.\+\-]+@[\w\-]+(\.[a-zA-Z]{2,})+$"))
                throw new Exception("O e-mail informado não é válido.");

            Email = email.Trim();
        }

        public void SetTelefone(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                throw new Exception("O telefone não pode ser vazio ou composto por espaços em branco.");

            if (telefone.Length < 8 || telefone.Length > 20)
                throw new Exception("O telefone deve ter entre 8 e 20 caracteres.");

            if (!Regex.IsMatch(telefone.Trim(), @"^(\+55\s?)?(\(?\d{2}\)?)\s?9?\d{4}-?\d{4}$"))
                throw new Exception("O telefone informado não é válido. Formatos aceitos: (11) 99999-9999, 11999999999, +55 (11) 99999-9999.");

            Telefone = telefone.Trim();
        }

        public void SetSenha(string senha)
        {
            if(string.IsNullOrWhiteSpace(senha))
                throw new Exception("A senha não pode ser vazio ou composto por espaços em branco.");

            if(senha.Length < 9 || senha.Length > 150)
                throw new Exception("A quantidade de caracteres da senha deve ter entre 9 e 150 caracteres.");

            Senha = senha;
        }

        public void SetDataCadastro(DateTime dataCadastro)
        {
            if(dataCadastro == DateTime.MinValue)
                throw new Exception("Data não inserida");

            DataCadastro = dataCadastro;
        }
    }
}