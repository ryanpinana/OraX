using SQLite;

namespace OraX.Models
{
    public class TipoDb
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Nome { get; set; }

        public string ColoreHex { get; set; }  // es. "#7FFFD4"
    }
}
