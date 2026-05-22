using SQLite;
using OraX.Models;

namespace OraX.Services
{
    public class DatabaseService
    {
        SQLiteAsyncConnection database;

        public async Task Init()
        {
            if (database != null)
                return;

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "orax.db");
            database = new SQLiteAsyncConnection(dbPath);

            await database.CreateTableAsync<User>();
            await database.CreateTableAsync<AttivitaDb>();
            await database.CreateTableAsync<TipoDb>();
            await database.CreateTableAsync<Calendario>();
            await database.CreateTableAsync<CalendarioUtente>();
            await database.CreateTableAsync<RichiestaCondivisione>();
            await database.CreateTableAsync<Attivita>();
        }

        // -------------------------
        // User
        // -------------------------

        public async Task<int> RegistraUser(User user)
        {
            await Init();
            return await database.InsertAsync(user);
        }

        public async Task<User> GetUser(string username, string password)
        {
            await Init();
            return await database.Table<User>()
                .Where(u => u.Username == username && u.PasswordHash == password)
                .FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByUsername(string username)
        {
            await Init();

            username = username.Trim().ToLower();

            var utenti =
                await database
                .Table<User>()
                .ToListAsync();

            return utenti.FirstOrDefault(u =>
                u.Username.ToLower() == username);
        }

        public async Task UpdateUser(User user)
        {
            await Init();
            await database.UpdateAsync(user);
        }

        // -------------------------
        // Tipi
        // -------------------------

        public async Task<List<TipoDb>> GetTipi()
        {
            await Init();
            return await database.Table<TipoDb>().ToListAsync();
        }

        public async Task<int> SalvaTipo(TipoDb tipo)
        {
            await Init();
            return await database.InsertAsync(tipo);
        }

        // Inizializza i tipi di default se non esistono ancora
        public async Task InitTipiDefault()
        {
            await Init();
            var tipiEsistenti = await database.Table<TipoDb>().CountAsync();
            if (tipiEsistenti == 0)
            {
                await database.InsertAllAsync(new List<TipoDb>
                {
                    new TipoDb { Nome = "Scuola",  ColoreHex = "#7FFFD4" }, // Aquamarine
                    new TipoDb { Nome = "Casa",    ColoreHex = "#008B8B" }, // DarkCyan
                    new TipoDb { Nome = "Viaggi",  ColoreHex = "#FFD700" }, // Gold
                });
            }
        }

        // -------------------------
        // Attivita
        // -------------------------

        public async Task<List<AttivitaDb>> GetAttivitaByUser(string username)
        {
            await Init();
            return await database.Table<AttivitaDb>()
                .Where(a => a.Username == username)
                .ToListAsync();
        }

        public async Task<int> SalvaAttivita(AttivitaDb attivita)
        {
            await Init();
            return await database.InsertAsync(attivita);
        }

        public async Task EliminaAttivita(int id)
        {
            await Init();
            await database.DeleteAsync<AttivitaDb>(id);
        }
        public async Task<int> SalvaCalendario(Calendario calendario)
        {
            await Init();

            return await database.InsertAsync(calendario);
        }
        public async Task<int> InviaRichiesta(RichiestaCondivisione richiesta)
        {
            await Init();

            return await database.InsertAsync(richiesta);
        }
        public async Task<List<RichiestaCondivisione>> GetRichieste(string username)
        {
            await Init();

            return await database
                .Table<RichiestaCondivisione>()
                .Where(r =>
                    r.DestinatarioUsername == username &&
                    r.Stato == "In attesa")
                .ToListAsync();
        }
        public async Task<List<CalendarioUtente>>
    GetUtentiCalendario(
        int calendarioId)
        {
            await Init();

            return await database
                .Table<CalendarioUtente>()
                .Where(c =>
                    c.CalendarioId ==
                    calendarioId)
                .ToListAsync();
        }
        public async Task<List<Attivita>> GetAttivitaCalendario(int calendarioId)
        {
            await Init();

            return await database.Table<Attivita>()
                .Where(a => a.CalendarioId == calendarioId)
                .ToListAsync();
        }
        public async Task<List<Calendario>>
    GetCalendariUtente(
        string username)
        {
            await Init();

            var relazioni =
                await database
                .Table<CalendarioUtente>()
                .Where(c =>
                    c.Username ==
                    username)
                .ToListAsync();

            List<Calendario> calendari =
                new();

            foreach (var relazione in relazioni)
            {
                var calendario =
                    await database
                    .Table<Calendario>()
                    .Where(c =>
                        c.Id ==
                        relazione.CalendarioId)
                    .FirstOrDefaultAsync();

                if (calendario != null)
                    calendari.Add(calendario);
            }

            return calendari;
        }
        public async Task<int> AggiungiUtenteCalendario(
    CalendarioUtente utente)
        {
            await Init();

            return await database.InsertAsync(utente);
        }
        public async Task<int> AggiornaRichiesta(RichiestaCondivisione richiesta)
        {
            await Init();
            return await database.UpdateAsync(richiesta);
        }

        public async Task<Calendario?> GetCalendarioById(int id)
        {
            await Init();

            return await database
                .Table<Calendario>()
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UtenteGiaNelCalendario(int calendarioId, string username)
        {
            await Init();

            var relazione = await database
                .Table<CalendarioUtente>()
                .Where(c => c.CalendarioId == calendarioId && c.Username == username)
                .FirstOrDefaultAsync();

            return relazione != null;
        }
        public async Task<List<Attivita>> GetAttivitaUtente(string username)
        {
            await Init();

            return await database.Table<Attivita>()
                .Where(a => a.Username == username)
                .ToListAsync();
        }

        public async Task<List<Attivita>> GetAttivitaUtenteCalendario(
    int calendarioId,
    string username)
        {
            await Init();

            return await database.Table<Attivita>()
                .Where(a =>
                    a.CalendarioId == calendarioId &&
                    a.Username == username)
                .ToListAsync();
        }
    }
}
