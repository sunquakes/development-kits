using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace development_kits.Pages
{
    public partial class Base64ImagePage : Page
    {
        public Base64ImagePage()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }

        private void Decode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var base64 = InputText.Text ?? string.Empty;
                var bytes = Convert.FromBase64String(base64);
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(bytes);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                ResultImage.Source = bmp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ω‚¬Î ß∞‹: " + ex.Message);
            }
        }
    }
}