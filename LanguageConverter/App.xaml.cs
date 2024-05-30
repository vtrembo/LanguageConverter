using System.Windows;

namespace LanguageConverter
{
    public partial class App : Application
    {
        private readonly NotifyIconManager notifyIconManager;

        public App()
        {
            notifyIconManager = new NotifyIconManager();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow();

            notifyIconManager.Initialize();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIconManager.Dispose();
            base.OnExit(e);
        }
    }
}