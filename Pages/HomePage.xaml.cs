using System.Windows;
using System.Windows.Controls;

namespace development_kits.Pages
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void BtnMd5_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Md5Page());
        }

        private void BtnBarcode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new BarcodePage());
        }

        private void BtnQr_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new QrPage());
        }

        private void BtnBase64ToImage_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Base64ImagePage());
        }

        private void BtnJsonFormat_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new JsonFormatPage());
        }

        private void BtnImageToBase64_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ImageToBase64Page());
        }
    }
}