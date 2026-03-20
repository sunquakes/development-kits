using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DevTools.Helpers;
using DevTools.Resources;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.MessageBox;

namespace DevTools.Pages
{
    public partial class Md5Page : Page
    {
        public Md5Page()
        {
            InitializeComponent();
            LoadState();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            SaveState();
            NavigationService?.Navigate(new HomePage());
        }

        private void SaveState()
        {
            var state = new Dictionary<string, string>
            {
                { "InputText", InputText.Text ?? string.Empty },
                { "Out32Lower", Out32Lower.Text ?? string.Empty },
                { "Out32Upper", Out32Upper.Text ?? string.Empty },
                { "Out16Lower", Out16Lower.Text ?? string.Empty },
                { "Out16Upper", Out16Upper.Text ?? string.Empty }
            };
            PageStateManager.SavePageState(this, state);
        }

        private void LoadState()
        {
            var state = PageStateManager.GetPageState(this);
            if (state != null)
            {
                InputText.Text = state.GetValueOrDefault("InputText", string.Empty);
                Out32Lower.Text = state.GetValueOrDefault("Out32Lower", string.Empty);
                Out32Upper.Text = state.GetValueOrDefault("Out32Upper", string.Empty);
                Out16Lower.Text = state.GetValueOrDefault("Out16Lower", string.Empty);
                Out16Upper.Text = state.GetValueOrDefault("Out16Upper", string.Empty);
            }
        }

        private void Compute_Click(object sender, RoutedEventArgs e)
        {
            var input = InputText.Text ?? string.Empty;
            using var md5 = MD5.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = md5.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));

            var hexLower = sb.ToString();
            var hexUpper = hexLower.ToUpperInvariant();

            Out32Lower.Text = hexLower;
            Out32Upper.Text = hexUpper;

            if (hexLower.Length >= 24)
            {
                var mid16Lower = hexLower.Substring(8, 16);
                var mid16Upper = mid16Lower.ToUpperInvariant();
                Out16Lower.Text = mid16Lower;
                Out16Upper.Text = mid16Upper;
            }
            else
            {
                Out16Lower.Text = string.Empty;
                Out16Upper.Text = string.Empty;
            }
        }

        private void Copy32Lower_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Out32Lower.Text))
            {
                MessageBox.Show(Strings.ComputeFirst, Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            ClipboardHelper.CopyWithFeedback(Out32Lower.Text, (Button)sender);
        }
        private void Copy32Upper_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Out32Upper.Text))
            {
                MessageBox.Show(Strings.ComputeFirst, Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            ClipboardHelper.CopyWithFeedback(Out32Upper.Text, (Button)sender);
        }
        private void Copy16Lower_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Out16Lower.Text))
            {
                MessageBox.Show(Strings.ComputeFirst, Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            ClipboardHelper.CopyWithFeedback(Out16Lower.Text, (Button)sender);
        }
        private void Copy16Upper_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Out16Upper.Text))
            {
                MessageBox.Show(Strings.ComputeFirst, Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            ClipboardHelper.CopyWithFeedback(Out16Upper.Text, (Button)sender);
        }
    }
}
