using SQLite;

namespace OraX.Models
{
    // Modello per il database — usa tipi semplici compatibili con SQLite
    public class AttivitaDb
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Username { get; set; }   // collega l'attività all'utente

        public string Titolo { get; set; }

        public DateTime Data { get; set; }

        public DateTime? DataFine { get; set; }

        public string ColoreHex { get; set; }  // es. "#FF0000" — Color non è supportato da SQLite

        public int? TipoId { get; set; }       // FK verso TipoDb

        public string Note { get; set; }
        public bool NotificheAttive { get; set; }

        public int MinutiPreavviso { get; set; }

        public int CalendarioId { get; set; }
        public string Colore { get; set; } = "";
    }
}
