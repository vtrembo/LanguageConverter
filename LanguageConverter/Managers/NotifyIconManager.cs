using System.Drawing;
using System.Windows.Forms;

namespace LanguageConverter
{
    public class NotifyIconManager : IDisposable
    {
        private const string selectAllBeforeConvertingToolTip = "Enable this function to automatically convert all text in the text field into Ukrainian language. Disable if you want to convert only selected text.";
        private const string selectAllBeforeConvertingText = "Select all before converting";

        private const string changeShortcutText = "Change shortcut";
        private const string exitText = "Exit";
        private const string launchOnStartupText = "Launch on startup";

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

        private ToolStripMenuItem selectedMenuItem;

        public NotifyIconManager()
        {
            hotkeyManager = new HotkeyManager();

            notifyIcon = new NotifyIcon
            {
                Icon = new Icon("Resources/languageConverter.ico"),
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };
        }

        public void Initialize()
        {
            hotkeyManager.Initialize();

            AddContextMenuItem(launchOnStartupText, OnLaunchOnStartupClicked);
            AddContextMenuItem(selectAllBeforeConvertingText, OnSelectAllBeforeConvertingClicked);
            AddContextMenuItem(changeShortcutText, null, CreateShortcutMenuItems());
            AddContextMenuItem(exitText, OnExitMenuItemClicked);

            SetLaunchOnStartupCheckedState();
            HighlightCurrentHotkey();
            SetSelectAllBeforeConvertingCheckedState();
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

        private void UpdateSelectedMenuItem(ToolStripMenuItem menuItem)
        {
            if (selectedMenuItem != null)
            {
                selectedMenuItem.Checked = false;
            }
            selectedMenuItem = menuItem;

            selectedMenuItem.Checked = true;
        }

        private void HighlightCurrentHotkey()
        {
            var storedHotKey = hotkeyManager.GetStoredHotKey();

            var changeShortcutItem = notifyIcon.ContextMenuStrip.Items.OfType<ToolStripMenuItem>()
                .FirstOrDefault(item => item.Text == changeShortcutText);

            foreach (ToolStripMenuItem menuItem in changeShortcutItem.DropDownItems)
            {
                if (menuItem.Tag is string keyName && keyName == MapHotKeyToHotKeyName(storedHotKey))
                {
                    UpdateSelectedMenuItem(menuItem);
                    break;
                }
            }
        }

        private string MapHotKeyToHotKeyName(uint hotkey)
        {
            foreach (var pair in functionKeyMapping)
            {
                if (pair.Value == hotkey)
                {
                    return pair.Key;
                }
            }
            return string.Empty;
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

                HighlightCurrentHotkey();
            }

        }

        private void OnSelectAllBeforeConvertingClicked(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                menuItem.Checked = !menuItem.Checked;
                SaveSelectAllBeforeConvertingSetting(menuItem.Checked);
            }
        }

        private void OnLaunchOnStartupClicked(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                menuItem.Checked = !menuItem.Checked;
                SaveLaunchOnStartupSetting(menuItem.Checked);
                StartupManager.SetStartup(menuItem.Checked);
            }
        }

        private void SaveLaunchOnStartupSetting(bool isChecked)
        {
            Settings.Default.LaunchOnStartup = isChecked;
            Settings.Default.Save();
        }

        private void SetLaunchOnStartupCheckedState()
        {
            var launchOnStartupItem = notifyIcon.ContextMenuStrip.Items.OfType<ToolStripMenuItem>()
                .FirstOrDefault(item => item.Text == launchOnStartupText);
            if (launchOnStartupItem != null)
            {
                launchOnStartupItem.Checked = Settings.Default.LaunchOnStartup;
            }
        }

        private void SaveSelectAllBeforeConvertingSetting(bool isChecked)
        {
            Settings.Default.SelectAllBeforeConverting = isChecked;
            Settings.Default.Save();
        }

        private void SetSelectAllBeforeConvertingCheckedState()
        {
            var selectAllItem = notifyIcon.ContextMenuStrip.Items.OfType<ToolStripMenuItem>()
                .FirstOrDefault(item => item.Text == selectAllBeforeConvertingText);
            if (selectAllItem != null)
            {
                selectAllItem.Checked = Settings.Default.SelectAllBeforeConverting;
                selectAllItem.ToolTipText = selectAllBeforeConvertingToolTip;
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