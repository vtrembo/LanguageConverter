using System.Drawing;
using System.Windows.Forms;

namespace LanguageConverter
{
    public class NotifyIconManager : IDisposable
    {
        private readonly NotifyIcon notifyIcon;

        public NotifyIconManager()
        {
            notifyIcon = new NotifyIcon
            {
                Icon = new Icon("Resources/tools.ico"),
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };
        }

        public void Initialize()
        {
            AddContextMenuItem("Exit", OnExitMenuItemClicked);
        }

        public void AddContextMenuItem(string text, EventHandler onClick)
        {
            var menuItem = new ToolStripMenuItem(text);
            menuItem.Click += onClick;
            notifyIcon.ContextMenuStrip.Items.Add(menuItem);
        }

        private void OnExitMenuItemClicked(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void Dispose()
        {
            notifyIcon.Dispose();
        }
    }
}