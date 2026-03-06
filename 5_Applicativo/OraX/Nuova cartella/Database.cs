using SQLite;

public class DatabaseService
{
    SQLiteAsyncConnection database;

    public async Task Init()
    {
        if (database != null)
            return;

        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "calendario.db");

        database = new SQLiteAsyncConnection(databasePath);

        await database.CreateTableAsync<Utente>();
        await database.CreateTableAsync<Attivita>();
    }
    public Task<int> AggiungiUtente(Utente utente)
    {
        return database.InsertAsync(utente);
    }
}