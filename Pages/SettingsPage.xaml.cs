using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Forms;
using DevTools.Resources;
using MessageBox = System.Windows.MessageBox;
using Application = System.Windows.Application;
using CheckBox = System.Windows.Controls.CheckBox;

namespace DevTools.Pages
{
    public partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private const string ShortcutName = "DevTools.lnk";
        private const string AppName = "DevTools.exe";
        private string _hotkeyText = "";

        public SettingsPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadSettings();
        }

        public string HotkeyText
        {
            get => _hotkeyText;
            set
            {
                _hotkeyText = value;
                OnPropertyChanged(nameof(HotkeyText));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadSettings()
        {
            AutoStartCheckBox.IsChecked = IsAutoStartEnabled();
            TrayShowHideCheckBox.IsChecked = IsTrayShowHideEnabled();
            
            var hotkey = Properties.Settings.Default.Hotkey;
            if (!string.IsNullOrWhiteSpace(hotkey))
            {
                HotkeyText = hotkey;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }

        private void AutoStartCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SetAutoStart(true);
        }

        private void AutoStartCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetAutoStart(false);
        }

        private void TrayShowHideCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SetTrayShowHide(true);
        }

        private void TrayShowHideCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetTrayShowHide(false);
        }

        private void HotkeyTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
            
            var key = e.Key;
            if (key == Key.System)
            {
                key = e.SystemKey;
            }
            
            if (key == Key.Escape)
            {
                var hotkey = Properties.Settings.Default.Hotkey;
                HotkeyText = string.IsNullOrWhiteSpace(hotkey) ? "" : hotkey;
                HotkeyHintText.Visibility = Visibility.Collapsed;
                return;
            }
            
            if (key == Key.LeftShift || key == Key.RightShift || 
                key == Key.LeftCtrl || key == Key.RightCtrl || 
                key == Key.LeftAlt || key == Key.RightAlt || 
                key == Key.LWin || key == Key.RWin)
            {
                UpdateHotkeyDisplay();
                return;
            }
            
            if (key == Key.Left || key == Key.Right || 
                key == Key.Up || key == Key.Down)
            {
                return;
            }
            
            CompleteHotkeyRecording(key);
        }

        private void HotkeyTextBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            UpdateHotkeyDisplay();
        }

        private void HotkeyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            HotkeyHintText.Visibility = Visibility.Visible;
        }

        private void HotkeyTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            HotkeyHintText.Visibility = Visibility.Collapsed;
        }

        private void UpdateHotkeyDisplay()
        {
            var modifiers = "";
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                modifiers += "Ctrl+";
            }
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                modifiers += "Alt+";
            }
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                modifiers += "Shift+";
            }
            
            HotkeyText = modifiers;
        }

        private void CompleteHotkeyRecording(Key key)
        {
            var modifiers = "";
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                modifiers += "Ctrl+";
            }
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                modifiers += "Alt+";
            }
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                modifiers += "Shift+";
            }
            
            HotkeyText = modifiers + key.ToString();
            Properties.Settings.Default.Hotkey = HotkeyText;
            Properties.Settings.Default.Save();
            
            HotkeyHintText.Visibility = Visibility.Collapsed;
            
            RegisterHotkey();
        }

        private bool IsAutoStartEnabled()
        {
            try
            {
                var startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                var shortcutPath = Path.Combine(startupPath, ShortcutName);
                return File.Exists(shortcutPath);
            }
            catch
            {
                return false;
            }
        }

        private void SetAutoStart(bool enable)
        {
            try
            {
                var startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                var shortcutPath = Path.Combine(startupPath, ShortcutName);
                var appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                if (enable)
                {
                    CreateShortcut(shortcutPath, appPath, Strings.PageSettings);
                }
                else
                {
                    if (File.Exists(shortcutPath))
                    {
                        File.Delete(shortcutPath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.Error}: {ex.Message}", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsTrayShowHideEnabled()
        {
            try
            {
                return Properties.Settings.Default.TrayShowHide;
            }
            catch
            {
                return false;
            }
        }

        private void SetTrayShowHide(bool enable)
        {
            try
            {
                Properties.Settings.Default.TrayShowHide = enable;
                Properties.Settings.Default.Save();
                
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.UpdateTrayShowHide(enable);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.Error}: {ex.Message}", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterHotkey()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.RegisterHotkey(HotkeyText);
        }

        private void CreateShortcut(string shortcutPath, string targetPath, string description)
        {
            try
            {
                var shellType = Type.GetTypeFromProgID("WScript.Shell");
                if (shellType == null)
                {
                    MessageBox.Show(Strings.Error, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                dynamic shell = Activator.CreateInstance(shellType)!;
                if (shell == null)
                {
                    MessageBox.Show(Strings.Error, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                var shortcut = shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = targetPath;
                shortcut.Description = description;
                shortcut.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.Error}: {ex.Message}", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
