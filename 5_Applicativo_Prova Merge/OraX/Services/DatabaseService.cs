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
        }

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
    }
}