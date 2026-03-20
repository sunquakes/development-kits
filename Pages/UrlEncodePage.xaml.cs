using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using DevTools.Helpers;
using DevTools.Resources;
using MessageBox = System.Windows.MessageBox;

namespace DevTools.Pages
{
    public partial class UrlEncodePage : Page
    {
        public UrlEncodePage()
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
                { "OutputText", OutputText.Text ?? string.Empty }
            };
            PageStateManager.SavePageState(this, state);
        }

        private void LoadState()
        {
            var state = PageStateManager.GetPageState(this);
            if (state != null)
            {
                InputText.Text = state.GetValueOrDefault("InputText", string.Empty);
                OutputText.Text = state.GetValueOrDefault("OutputText", string.Empty);
            }
        }

        private void Encode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var input = InputText.Text;
                if (string.IsNullOrWhiteSpace(input))
                {
                    MessageBox.Show(Strings.InputEmpty, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var encoded = Uri.EscapeDataString(input);
                OutputText.Text = encoded;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.EncodeFailed}: {ex.Message}", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Decode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var input = InputText.Text;
                if (string.IsNullOrWhiteSpace(input))
                {
                    MessageBox.Show(Strings.InputEmpty, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var decoded = Uri.UnescapeDataString(input);
                OutputText.Text = decoded;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.DecodeFailed}: {ex.Message}", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CopyOutput_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var output = OutputText.Text;
                if (string.IsNullOrWhiteSpace(output))
                {
                    MessageBox.Show(Strings.OutputEmpty, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                System.Windows.Clipboard.SetText(output);
                MessageBox.Show(Strings.CopySuccess, Strings.Success, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.SaveFailed}: {ex.Message}", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
