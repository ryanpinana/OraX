using SQLite;

namespace OraX.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Cognome { get; set; }

        public DateTime DataNascita { get; set; }

        [Unique]
        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public DateTime DataRegistrazione { get; set; }
    }
}