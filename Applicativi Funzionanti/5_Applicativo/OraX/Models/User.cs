using SQLite;

namespace OraX.Models
{
    public class User
    {
        
        public string Nome { get; set; }

        public string Cognome { get; set; }

        public DateTime DataNascita { get; set; }

       
        [PrimaryKey, Unique]
        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string Email { get; set; }

        public string Telefono { get; set; }

        public DateTime DataRegistrazione { get; set; }
    }
}