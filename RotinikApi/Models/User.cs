using System;
using System.Text.RegularExpressions;

namespace RotinikApi.Models
{
    public class User
    {
        public int Id { get; protected set; }
        public string Name { get; protected set; }
        public string Email { get; protected set; }
        public string Phone { get; protected set; }
        public string Password { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        public User(string name, string email, string phone, string password)
        {
            SetName(name);
            SetEmail(email);
            SetPhone(phone);
            SetPassword(password);
            SetCreatedAt(DateTime.UtcNow);
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("Name cannot be empty or composed of whitespace.");

            if (name.Length < 3 || name.Length > 150)
                throw new Exception("Name must be between 3 and 150 characters.");

            Name = name;
        }

        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("Email cannot be empty or composed of whitespace.");

            if (email.Length < 6 || email.Length > 150)
                throw new Exception("Email must be between 6 and 150 characters.");

            if (!Regex.IsMatch(email.Trim(), @"^[\w\.\+\-]+@[\w\-]+(\.[a-zA-Z]{2,})+$"))
                throw new Exception("The provided email is invalid.");

            Email = email.Trim();
        }

        public void SetPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new Exception("Phone cannot be empty or composed of whitespace.");

            if (phone.Length < 8 || phone.Length > 20)
                throw new Exception("Phone must be between 8 and 20 characters.");

            if (!Regex.IsMatch(phone.Trim(), @"^(\+55\s?)?(\(?\d{2}\)?)\s?9?\d{4}-?\d{4}$"))
                throw new Exception("The provided phone number is invalid. Accepted formats: (11) 99999-9999, 11999999999, +55 (11) 99999-9999.");

            Phone = phone.Trim();
        }

        public void SetPassword(string password)
        {
            if(string.IsNullOrWhiteSpace(password))
                throw new Exception("Password cannot be empty or composed of whitespace.");

            if(password.Length < 9 || password.Length > 150)
                throw new Exception("Password must be between 9 and 150 characters.");

            Password = password;
        }

        public void SetCreatedAt(DateTime createdAt)
        {
            if(createdAt == DateTime.MinValue)
                throw new Exception("Date not provided.");

            CreatedAt = createdAt;
        }
    }
}