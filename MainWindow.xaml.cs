using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using DevTools.Resources;
using Application = System.Windows.Application;
using ContextMenuStrip = System.Windows.Forms.ContextMenuStrip;
using ToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem;
using ToolTipIcon = System.Windows.Forms.ToolTipIcon;

namespace DevTools
{
    public partial class MainWindow : Window
    {
        private NotifyIcon? _notifyIcon;
        private bool _firstClose = true;
        private bool _minimizeToTray;
        private bool _isClosing = false;

        public MainWindow()
        {
            InitializeComponent();
            Title = Strings.Toolbox;
            MainFrame.Navigate(new Pages.HomePage());
            
            LoadSettings();
            InitializeNotifyIcon();
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
            return IntPtr.Zero;
        }

        protected override void OnClosed(EventArgs e)
        {
            _notifyIcon?.Dispose();
            base.OnClosed(e);
        }
    }
}
