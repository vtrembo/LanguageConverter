using System.Windows;
using System.Windows.Interop;

namespace LanguageConverter
{
    public class HotkeyManager : IDisposable
    {
        private readonly ClipboardManager clipboardManager;
        private readonly InputSimulator inputSimulator;
        private readonly TextConverter textConverter;

        private IntPtr windowHandle;

        public const int HotkeyId = 1;
        public const int WM_HOTKEY = 0x0312;

        public HotkeyManager()
        {
            clipboardManager = new ClipboardManager();
            inputSimulator = new InputSimulator();
            textConverter = new TextConverter();
        }

        public void Initialize()
        {
            windowHandle = new WindowInteropHelper(Application.Current.MainWindow).EnsureHandle();
            var storedHotkey = Settings.Default.LastSelectedHotkey;

            RegisterHotKey(windowHandle, HotkeyId, 0, storedHotkey != 0 ? storedHotkey : 0x73);

            HwndSource.FromHwnd(windowHandle)?.AddHook(HwndHook);
        }

        private void RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk)
        {
            NativeMethods.RegisterHotKey(hWnd, id, fsModifiers, vk);
        }

        private void UnregisterHotKey(IntPtr hWnd, int id)
        {
            NativeMethods.UnregisterHotKey(hWnd, id);
        }

        public void ChangeHotkey(uint newVk)
        {
            UnregisterHotKey(windowHandle, HotkeyId);
            RegisterHotKey(windowHandle, HotkeyId, 0, newVk);
            SaveLastSelectedHotkey(newVk);
        }

        private void SaveLastSelectedHotkey(uint hotkey)
        {
            Settings.Default.LastSelectedHotkey = hotkey;
            Settings.Default.Save();
        }

        private IntPtr HwndHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (id == HotkeyId)
                {
                    OnHotKeyPressed();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        private void OnHotKeyPressed()
        {
            // Simulate changing the selected text to Ukrainian
            inputSimulator.SendCtrlC(); // Copy the selected text to clipboard

            // Wait a moment for the clipboard operation to complete
            Thread.Sleep(100);

            // Get the copied text from clipboard
            string copiedText = clipboardManager.GetClipboardText();

            // Convert the copied text to Ukrainian
            string convertedText = textConverter.ConvertToUkrainian(copiedText);

            // Set the clipboard to the converted text
            clipboardManager.SetClipboardText(convertedText);

            // Paste the converted text
            inputSimulator.SendCtrlV();
        }

        public void Dispose()
        {
            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                UnregisterHotKey(windowHandle, HotkeyId);
            }
        }
    }

}
