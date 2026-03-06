public partial class App : Application
{
    public static DatabaseService Database { get; private set; }

    public App()
    {
        InitializeComponent();

        Database = new DatabaseService();
        Database.Init();

        MainPage = new AppShell();
    }
}