using SQLite;

public class Attivita
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int CalendarioId { get; set; }

    public int CreatoreId { get; set; }

    public string Titolo { get; set; }

    public int Priorita { get; set; }

    public string Luogo { get; set; }

    public int CategoriaId { get; set; }

    public DateTime Scadenza { get; set; }

    public string Note { get; set; }

    public bool Completata { get; set; }
}