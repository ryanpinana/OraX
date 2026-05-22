using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OraX.Models
{
    public class RichiestaCondivisione
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string MittenteUsername { get; set; } = "";

        public string DestinatarioUsername { get; set; } = "";

        public int CalendarioId { get; set; }

        public string Stato { get; set; } = "In attesa";
    }
}
