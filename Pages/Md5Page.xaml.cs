using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using development_kits.Helpers;

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

        private void Copy32Lower_Click(object sender, RoutedEventArgs e) => ClipboardHelper.CopyWithFeedback(Out32Lower.Text, (Button)sender);
        private void Copy32Upper_Click(object sender, RoutedEventArgs e) => ClipboardHelper.CopyWithFeedback(Out32Upper.Text, (Button)sender);
        private void Copy16Lower_Click(object sender, RoutedEventArgs e) => ClipboardHelper.CopyWithFeedback(Out16Lower.Text, (Button)sender);
        private void Copy16Upper_Click(object sender, RoutedEventArgs e) => ClipboardHelper.CopyWithFeedback(Out16Upper.Text, (Button)sender);
    }
}