using OraX.Services;

namespace OraX
{
    public partial class App : Application
    {
        public App(DatabaseService db)
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}
