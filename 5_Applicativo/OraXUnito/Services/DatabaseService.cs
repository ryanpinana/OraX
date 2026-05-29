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

            // se l'app è già installata, aggiungo le colonne nuove senza rifare tutto il db
            await AggiungiColonnaSeManca("AttivitaDb", "Completata", "INTEGER DEFAULT 0");
            await AggiungiColonnaSeManca("AttivitaDb", "NotificaInviata", "INTEGER DEFAULT 0");
        }

        // utenti
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
            var utenti = await database.Table<User>().ToListAsync();

            return utenti.FirstOrDefault(u => u.Username.ToLower() == username);
        }

        public async Task UpdateUser(User user)
        {
            await Init();
            await database.UpdateAsync(user);
        }

        // tipi attività
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

        public async Task InitTipiDefault()
        {
            await Init();

            int tipiEsistenti = await database.Table<TipoDb>().CountAsync();
            if (tipiEsistenti != 0)
                return;

            await database.InsertAllAsync(new List<TipoDb>
            {
                new TipoDb { Nome = "Scuola", ColoreHex = "#7FFFD4" },
                new TipoDb { Nome = "Casa", ColoreHex = "#008B8B" },
                new TipoDb { Nome = "Viaggi", ColoreHex = "#FFD700" }
            });
        }

        // attività
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
            await database.InsertAsync(attivita);
            return attivita.Id;
        }

        public async Task EliminaAttivita(int id)
        {
            await Init();
            await database.DeleteAsync<AttivitaDb>(id);
        }

        public async Task<int> AggiornaAttivita(AttivitaDb attivita)
        {
            await Init();

            // Prima recupero la riga reale dal database.
            // Così siamo sicuri di aggiornare proprio l'attività esistente
            // e non un oggetto scollegato che SQLite potrebbe non salvare bene.
            var esistente = await database.Table<AttivitaDb>()
                .Where(a => a.Id == attivita.Id)
                .FirstOrDefaultAsync();

            if (esistente == null)
                return 0;

            esistente.Username = attivita.Username;
            esistente.Titolo = attivita.Titolo;
            esistente.Data = attivita.Data;
            esistente.DataFine = attivita.DataFine;
            esistente.ColoreHex = attivita.ColoreHex;
            esistente.TipoId = attivita.TipoId;
            esistente.Note = attivita.Note;
            esistente.NotificheAttive = attivita.NotificheAttive;
            esistente.MinutiPreavviso = attivita.MinutiPreavviso;
            esistente.CalendarioId = attivita.CalendarioId;
            esistente.Completata = attivita.Completata;
            esistente.NotificaInviata = attivita.NotificaInviata;

            return await database.UpdateAsync(esistente);
        }

        public async Task<List<AttivitaDb>> GetAttivitaUtente(string username)
        {
            await Init();

            return await database.Table<AttivitaDb>()
                .Where(a => a.Username == username)
                .ToListAsync();
        }

        public async Task<List<AttivitaDb>> GetAttivitaCalendario(int calendarioId)
        {
            await Init();

            return await database.Table<AttivitaDb>()
                .Where(a => a.CalendarioId == calendarioId)
                .ToListAsync();
        }

        public async Task<List<AttivitaDb>> GetAttivitaUtenteCalendario(int calendarioId, string username)
        {
            await Init();

            return await database.Table<AttivitaDb>()
                .Where(a => a.CalendarioId == calendarioId && a.Username == username)
                .ToListAsync();
        }

        public async Task ImpostaCompletata(int attivitaId, bool completata)
        {
            await Init();

            var attivita = await database.Table<AttivitaDb>()
                .Where(a => a.Id == attivitaId)
                .FirstOrDefaultAsync();

            if (attivita == null)
                return;

            attivita.Completata = completata;
            await database.UpdateAsync(attivita);
        }

        // calendari
        public async Task<int> SalvaCalendario(Calendario calendario)
        {
            await Init();
            await database.InsertAsync(calendario);
            return calendario.Id;
        }

        public async Task<int> AggiornaCalendario(Calendario calendario)
        {
            await Init();
            return await database.UpdateAsync(calendario);
        }

        public async Task<Calendario?> GetCalendarioById(int id)
        {
            await Init();

            return await database.Table<Calendario>()
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Calendario>> GetCalendariUtente(string username)
        {
            await Init();

            var relazioni = await database.Table<CalendarioUtente>()
                .Where(c => c.Username == username)
                .ToListAsync();

            List<Calendario> calendari = new();

            foreach (var relazione in relazioni)
            {
                var calendario = await database.Table<Calendario>()
                    .Where(c => c.Id == relazione.CalendarioId)
                    .FirstOrDefaultAsync();

                if (calendario != null)
                    calendari.Add(calendario);
            }

            return calendari;
        }

        public async Task<int> AggiungiUtenteCalendario(CalendarioUtente utente)
        {
            await Init();
            return await database.InsertAsync(utente);
        }

        public async Task<List<CalendarioUtente>> GetUtentiCalendario(int calendarioId)
        {
            await Init();

            return await database.Table<CalendarioUtente>()
                .Where(c => c.CalendarioId == calendarioId)
                .ToListAsync();
        }

        public async Task<bool> UtenteGiaNelCalendario(int calendarioId, string username)
        {
            await Init();

            username = username.Trim().ToLower();
            var relazioni = await database.Table<CalendarioUtente>().ToListAsync();

            return relazioni.Any(c =>
                c.CalendarioId == calendarioId &&
                c.Username.Trim().ToLower() == username);
        }

        // richieste di condivisione calendario
        public async Task<int> InviaRichiesta(RichiestaCondivisione richiesta)
        {
            await Init();
            return await database.InsertAsync(richiesta);
        }

        public async Task<int> AggiornaRichiesta(RichiestaCondivisione richiesta)
        {
            await Init();
            return await database.UpdateAsync(richiesta);
        }

        public async Task<List<RichiestaCondivisione>> GetRichieste(string username)
        {
            await Init();

            username = username.Trim().ToLower();
            var richieste = await database.Table<RichiestaCondivisione>().ToListAsync();

            return richieste
                .Where(r =>
                    r.DestinatarioUsername.Trim().ToLower() == username &&
                    r.Stato == "In attesa")
                .ToList();
        }

        public async Task<bool> RichiestaPendenteEsiste(int calendarioId, string username)
        {
            await Init();

            username = username.Trim().ToLower();
            var richieste = await database.Table<RichiestaCondivisione>().ToListAsync();

            return richieste.Any(r =>
                r.CalendarioId == calendarioId &&
                r.DestinatarioUsername.Trim().ToLower() == username &&
                r.Stato == "In attesa");
        }

        // notifiche: prendo solo quelle che devono ancora partire
        public async Task<List<AttivitaDb>> GetAttivitaPerNotifiche()
        {
            await Init();

            return await database.Table<AttivitaDb>()
                .Where(a =>
                    a.NotificheAttive == true &&
                    a.NotificaInviata == false &&
                    a.Completata == false)
                .ToListAsync();
        }

        public async Task SegnaNotificaInviata(int attivitaId)
        {
            await Init();

            var attivita = await database.Table<AttivitaDb>()
                .Where(a => a.Id == attivitaId)
                .FirstOrDefaultAsync();

            if (attivita == null)
                return;

            attivita.NotificaInviata = true;
            await database.UpdateAsync(attivita);
        }

        async Task AggiungiColonnaSeManca(string tabella, string colonna, string definizioneSql)
        {
            var colonne = await database.QueryAsync<PragmaTableInfo>($"PRAGMA table_info({tabella})");

            if (!colonne.Any(c => c.name == colonna))
                await database.ExecuteAsync($"ALTER TABLE {tabella} ADD COLUMN {colonna} {definizioneSql}");
        }

        class PragmaTableInfo
        {
            public string name { get; set; } = "";
        }
    }
}
