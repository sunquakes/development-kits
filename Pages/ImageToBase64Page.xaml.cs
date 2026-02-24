using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace development_kits.Pages
{
    public partial class ImageToBase64Page : Page
    {
        public ImageToBase64Page()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Image files|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.webp|All files|*.*";
            var ok = dlg.ShowDialog();
            if (ok != true) return;

            try
            {
                var path = dlg.FileName;
                var bytes = File.ReadAllBytes(path);

                using var ms = new MemoryStream(bytes);
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = ms;
                bmp.EndInit();
                bmp.Freeze();

                ResultImage.Source = bmp;
                ImageDimensionsText.Text = $"{bmp.PixelWidth} x {bmp.PixelHeight}";
                ImageSizeText.Text = FormatBytes(bytes.Length);

                var base64 = Convert.ToBase64String(bytes);
                if (IncludeDataUriCheck.IsChecked == true)
                {
                    var mime = GetMimeFromExtension(Path.GetExtension(path));
                    Base64Text.Text = $"data:{mime};base64,{base64}";
                }
                else
                {
                    Base64Text.Text = base64;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载图片失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CopyBase64_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(Base64Text.Text ?? string.Empty);
            }
            catch
            {
                // ignore
            }
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            double value = bytes;
            string[] units = { "KB", "MB", "GB", "TB" };
            int unit = -1;
            while (value >= 1024 && unit < units.Length - 1)
            {
                value /= 1024;
                unit++;
            }
            if (unit < 0) unit = 0;
            return $"{value:0.##} {units[unit]}";
        }

        private static string GetMimeFromExtension(string? ext)
        {
            if (string.IsNullOrEmpty(ext)) return "application/octet-stream";
            ext = ext.Trim().ToLowerInvariant();
            return ext switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".bmp" => "image/bmp",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream",
            };
        }
    }
}
