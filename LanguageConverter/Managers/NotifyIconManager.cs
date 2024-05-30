using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media.Animation;

namespace LanguageConverter
{
    public class NotifyIconManager : IDisposable
    {
        private readonly HotkeyManager hotkeyManager;

        private readonly NotifyIcon notifyIcon;

        private readonly Dictionary<string, uint> functionKeyMapping = new Dictionary<string, uint>
        {
            { "F1", 0x70 },
            { "F2", 0x71 },
            { "F3", 0x72 },
            { "F4", 0x73 },
            { "F5", 0x74 },
            { "F6", 0x75 },
            { "F7", 0x76 },
            { "F8", 0x77 },
            { "F9", 0x78 },
            { "F10", 0x79 },
            { "F11", 0x7A },
            { "F12", 0x7B }
        };

        public NotifyIconManager()
        {
            hotkeyManager = new HotkeyManager();

            notifyIcon = new NotifyIcon
            {
                Icon = new Icon("Resources/tools.ico"),
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };
        }

        public void Initialize()
        {
            hotkeyManager.Initialize();

            AddContextMenuItem("Change shortcut", null, CreateShortcutMenuItems());
            AddContextMenuItem("Exit", OnExitMenuItemClicked);
        }
        private ToolStripMenuItem[] CreateShortcutMenuItems()
        {
            var shortcutItems = new ToolStripMenuItem[12];
            for (int i = 0; i < 12; i++)
            {
                var item = new ToolStripMenuItem($"F{i + 1}");
                item.Tag = $"F{i+1}";
                item.Click += OnShortcutMenuItemClicked;
                shortcutItems[i] = item;
            }
            return shortcutItems;
        }

        public void AddContextMenuItem(string text, EventHandler onClick, params ToolStripMenuItem[] subItems)
        {
            var menuItem = new ToolStripMenuItem(text);

            if (onClick != null)
            {
                menuItem.Click += onClick;
            }

            if (subItems != null)
            {
                menuItem.DropDownItems.AddRange(subItems);
            }
            notifyIcon.ContextMenuStrip.Items.Add(menuItem);
        }

        private void OnShortcutMenuItemClicked(object sender, EventArgs e)
        {

            if (sender is ToolStripMenuItem menuItem && menuItem.Tag is string keyName)
            {
                if (functionKeyMapping.TryGetValue(keyName, out uint keyCode))
                {
                    hotkeyManager.ChangeHotkey(keyCode);
                }
            }

        }

        private void OnExitMenuItemClicked(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void Dispose()
        {
            hotkeyManager.Dispose();
            notifyIcon.Dispose();
        }
    }
}