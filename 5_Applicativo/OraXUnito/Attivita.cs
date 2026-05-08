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
    }
}
