using System.Windows;
using System.Windows.Controls;
using System.Text.Json;

namespace development_kits.Pages
{
    public partial class JsonFormatPage : Page
    {
        public JsonFormatPage()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }

        private void Format_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var json = InputText.Text ?? string.Empty;
                using var doc = JsonDocument.Parse(json);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var formatted = JsonSerializer.Serialize(doc.RootElement, options);
                OutputText.Text = formatted;
            }
            catch (JsonException ex)
            {
                MessageBox.Show("JSON Ω‚Œˆ ß∞‹: " + ex.Message);
            }
        }
    }
}