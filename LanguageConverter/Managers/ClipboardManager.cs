using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace LanguageConverter
{
    public class ClipboardManager
    {

        private static Process GetProcessLockingClipboard()
        {
            int processId;
            NativeMethods.GetWindowThreadProcessId(NativeMethods.GetOpenClipboardWindow(), out processId);
            return Process.GetProcessById(processId);
        }

        private bool IsClipboardBusy()
        {
            return GetProcessLockingClipboard().ProcessName != "Idle";
        }

        public string GetClipboardText()
        {
            string clipboardText = string.Empty;
            int retries = 10;
            while (retries > 0)
            {
                try
                {
                    if (IsClipboardBusy())
                    {
                        NativeMethods.CloseClipboard();
                    }
                    clipboardText = Clipboard.GetText();
                    NativeMethods.CloseClipboard(); 
                    break;
                }
                catch (COMException)
                {
                    retries--;
                    Thread.Sleep(100);
                }
            }
            return clipboardText;
        }

        public void SetClipboardText(string text)
        {
            int retries = 10;
            while (retries > 0)
            {
                try
                {
                    Clipboard.Clear(); // Clear the clipboard before setting text
                    Clipboard.SetText(text);
                    NativeMethods.CloseClipboard(); // Ensure clipboard is closed
                    break;
                }
                catch (COMException)
                {
                    retries--;
                    Thread.Sleep(100);
                }
            }
        }
    }
}
