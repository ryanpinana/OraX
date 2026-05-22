namespace OraX
{
    public class Attivita
    {
        public int Id { get; set; }          // corrisponde all'Id nel DB
        public string Titolo { get; set; }
        public DateTime Data { get; set; }
        public DateTime? DataFine { get; set; }
        public Color Colore { get; set; }
        public Tipo? Tipo { get; set; }
        public string Note { get; set; }
        public bool NotificheAttive { get; set; }
        public int MinutiPreavviso { get; set; }
        public string Username { get; set; } = "";
        public int CalendarioId { get; set; }
        public bool Completata { get; set; }

    }
}
