using Microsoft.Win32;
using System.Windows.Forms;

public static class StartupManager
{
    private const string StartupKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
    private const string AppName = "LanguageConverter";

    public static void SetStartup(bool enable)
    {
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true))
        {
            if (enable)
            {
                key.SetValue(AppName, Application.ExecutablePath);
            }
            else
            {
                key.DeleteValue(AppName, false);
            }
        }
    }

    public static bool IsStartupEnabled()
    {
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, false))
        {
            if (key != null)
            {
                string value = (string)key.GetValue(AppName);
                return !string.IsNullOrEmpty(value);
            }
            return false;
        }
    }
}