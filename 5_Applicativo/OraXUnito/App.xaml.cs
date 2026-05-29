using OraX.Services;

namespace OraX
{
    public partial class App : Application
    {
        private readonly NotificationService notificationService;

        public App(DatabaseService db, NotificationService notificationService)
        {
            InitializeComponent();

            this.notificationService = notificationService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

            notificationService.Avvia();

            return window;
        }
    }
}
