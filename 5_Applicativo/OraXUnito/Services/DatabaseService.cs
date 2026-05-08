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
            return await database.Table<User>()
                .Where(u => u.Username == username)
                .FirstOrDefaultAsync();
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
    }
}
