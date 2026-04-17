using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
using System.Text;

namespace OraX.Services
{
    public static class PasswordHelper
    {

        public static string HashPassword(string password)
        {
            
            using var sha = SHA256.Create();

            
            byte[] bytes = Encoding.UTF8.GetBytes(password);

            
            byte[] hash = sha.ComputeHash(bytes);

            
            return Convert.ToBase64String(hash);
        }

    }
}