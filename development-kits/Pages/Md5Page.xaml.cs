using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace development_kits.Pages
{
    public partial class Md5Page : Page
    {
        public Md5Page()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
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
            OutputText.Text = sb.ToString();
        }
    }
}