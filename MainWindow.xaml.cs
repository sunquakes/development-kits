using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using DevTools.Properties;
using DevTools.Resources;
using Application = System.Windows.Application;
using ContextMenuStrip = System.Windows.Forms.ContextMenuStrip;
using ToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem;
using ToolTipIcon = System.Windows.Forms.ToolTipIcon;

namespace DevTools
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int MOD_ALT = 0x0001;
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_SHIFT = 0x0004;
        private const int MOD_WIN = 0x0008;
        private const int WM_HOTKEY = 0x0312;

        private NotifyIcon? _notifyIcon;
        private bool _firstClose = true;
        private bool _minimizeToTray;
        private bool _isClosing = false;
        private bool _trayShowHide = false;
        private int _hotkeyId = 9000;
        private string? _currentHotkey;

        public MainWindow()
        {
            InitializeComponent();
            Title = Strings.Toolbox;
            MainFrame.Navigate(new Pages.HomePage());
            
            LoadSettings();
            InitializeNotifyIcon();
            RegisterCurrentHotkey();
        }

        private void LoadSettings()
        {
            try
            {
                var minimizeSetting = ConfigurationManager.AppSettings["MinimizeToTray"];
                _minimizeToTray = minimizeSetting == "true";
            }
            catch
            {
                _minimizeToTray = false;
            }
        }

        private void SaveSettings()
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings["MinimizeToTray"] == null)
                {
                    config.AppSettings.Settings.Add("MinimizeToTray", _minimizeToTray.ToString().ToLower());
                }
                else
                {
                    config.AppSettings.Settings["MinimizeToTray"].Value = _minimizeToTray.ToString().ToLower();
                }
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch
            {
            }
        }

        private void InitializeNotifyIcon()
        {
            Icon? icon = LoadIcon();

            _notifyIcon = new NotifyIcon
            {
                Icon = icon ?? SystemIcons.Application,
                Text = Strings.Toolbox,
                Visible = true
            };

            _notifyIcon.MouseClick += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ShowWindow();
                }
            };

            var contextMenu = new ContextMenuStrip();
            var showItem = new ToolStripMenuItem(Strings.Show);
            showItem.Click += (sender, e) => ShowWindow();
            var exitItem = new ToolStripMenuItem(Strings.Exit);
            exitItem.Click += (sender, e) => ExitApplication();
            contextMenu.Items.Add(showItem);
            contextMenu.Items.Add(exitItem);
            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        public void UpdateTrayShowHide(bool enable)
        {
            _trayShowHide = enable;
            if (enable)
            {
                RegisterCurrentHotkey();
            }
            else
            {
                UnregisterHotkey();
            }
        }

        public void RegisterHotkey(string hotkey)
        {
            if (!_trayShowHide)
            {
                return;
            }

            UnregisterHotkey();

            if (string.IsNullOrEmpty(hotkey))
            {
                return;
            }

            _currentHotkey = hotkey;

            try
            {
                var (modifier, key) = ParseHotkey(hotkey);
                if (key != System.Windows.Forms.Keys.None)
                {
                    RegisterHotKeyInternal(_hotkeyId, modifier, key);
                }
            }
            catch
            {
            }
        }

        private void RegisterCurrentHotkey()
        {
            if (!_trayShowHide)
            {
                return;
            }

            var hotkey = Properties.Settings.Default.Hotkey;
            if (!string.IsNullOrEmpty(hotkey))
            {
                RegisterHotkey(hotkey);
            }
        }

        private void UnregisterHotkey()
        {
            if (!string.IsNullOrEmpty(_currentHotkey))
            {
                UnregisterHotKeyInternal(_hotkeyId);
                _currentHotkey = null;
            }
        }

        private (System.Windows.Forms.Keys modifier, System.Windows.Forms.Keys key) ParseHotkey(string hotkey)
        {
            var modifier = System.Windows.Forms.Keys.None;
            var key = System.Windows.Forms.Keys.None;

            var parts = hotkey.Split('+');
            foreach (var part in parts)
            {
                switch (part.Trim())
                {
                    case "Ctrl":
                        modifier |= System.Windows.Forms.Keys.Control;
                        break;
                    case "Alt":
                        modifier |= System.Windows.Forms.Keys.Alt;
                        break;
                    case "Shift":
                        modifier |= System.Windows.Forms.Keys.Shift;
                        break;
                    default:
                        if (Enum.TryParse(part, out System.Windows.Forms.Keys parsedKey))
                        {
                            key = parsedKey;
                        }
                        break;
                }
            }

            return (modifier, key);
        }

        private void RegisterHotKeyInternal(int id, System.Windows.Forms.Keys modifier, System.Windows.Forms.Keys key)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            uint fsModifiers = 0;

            if (modifier.HasFlag(System.Windows.Forms.Keys.Alt))
                fsModifiers |= MOD_ALT;
            if (modifier.HasFlag(System.Windows.Forms.Keys.Control))
                fsModifiers |= MOD_CONTROL;
            if (modifier.HasFlag(System.Windows.Forms.Keys.Shift))
                fsModifiers |= MOD_SHIFT;

            RegisterHotKey(hwnd, id, fsModifiers, (uint)key);
        }

        private void UnregisterHotKeyInternal(int id)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            UnregisterHotKey(hwnd, id);
        }

        private Icon? LoadIcon()
        {
            Icon? icon = null;

            try
            {
                var iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "logo.ico");
                if (System.IO.File.Exists(iconPath))
                {
                    icon = new Icon(iconPath);
                }
            }
            catch
            {
            }

            if (icon == null)
            {
                try
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var resourceNames = assembly.GetManifestResourceNames();
                    
                    foreach (var name in resourceNames)
                    {
                        if (name.EndsWith("logo.ico", StringComparison.OrdinalIgnoreCase))
                        {
                            using var stream = assembly.GetManifestResourceStream(name);
                            if (stream != null)
                            {
                                icon = new Icon(stream);
                                break;
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            if (icon == null)
            {
                try
                {
                    var exePath = Assembly.GetExecutingAssembly().Location;
                    if (!string.IsNullOrEmpty(exePath) && System.IO.File.Exists(exePath))
                    {
                        icon = System.Drawing.Icon.ExtractAssociatedIcon(exePath);
                    }
                }
                catch
                {
                }
            }

            return icon;
        }

        private void ShowWindow()
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        private void ExitApplication()
        {
            _isClosing = true;
            _notifyIcon?.Dispose();
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_isClosing)
            {
                return;
            }

            if (_firstClose)
            {
                e.Cancel = true;
                
                var dialog = new CloseOptionDialog
                {
                    Owner = this
                };
                
                if (dialog.ShowDialog() == true)
                {
                    _minimizeToTray = dialog.MinimizeToTray;
                    SaveSettings();
                    
                    if (_minimizeToTray)
                    {
                        _firstClose = false;
                        MinimizeToTray();
                    }
                    else
                    {
                        _isClosing = true;
                        Dispatcher.BeginInvoke(new Action(() => Close()));
                    }
                }
            }
            else if (_minimizeToTray)
            {
                e.Cancel = true;
                MinimizeToTray();
            }
        }

        private void MinimizeToTray()
        {
            Hide();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == App.ShowMeMessage)
            {
                ShowWindow();
                handled = true;
            }
            else if (msg == 0x0312) // WM_HOTKEY
            {
                if (_trayShowHide)
                {
                    if (IsVisible)
                    {
                        Hide();
                    }
                    else
                    {
                        ShowWindow();
                    }
                }
                handled = true;
            }
            return IntPtr.Zero;
        }

        protected override void OnClosed(EventArgs e)
        {
            _notifyIcon?.Dispose();
            base.OnClosed(e);
        }
    }
}
