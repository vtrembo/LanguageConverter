using System.Windows;

namespace LanguageConverter
{
    public partial class App : Application
    {
        private readonly NotifyIconManager notifyIconManager;
        private readonly HotkeyManager hotkeyManager;

        public App()
        {
            notifyIconManager = new NotifyIconManager();
            hotkeyManager = new HotkeyManager();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow();

            notifyIconManager.Initialize();
            hotkeyManager.Initialize();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            hotkeyManager.Dispose();
            notifyIconManager.Dispose();
            base.OnExit(e);
        }
    }
}