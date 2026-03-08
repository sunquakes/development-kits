using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DevTools.Pages
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
            var base64 = (InputText.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(base64))
            {
                MessageBox.Show("请输入Base64字符串", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                // 支持 data URI 前缀
                var commaIndex = base64.IndexOf(',');
                if (base64.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && commaIndex >= 0)
                {
                    base64 = base64.Substring(commaIndex + 1);
                }

                var bytes = Convert.FromBase64String(base64);

                using var ms = new MemoryStream(bytes);
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = ms;
                bmp.EndInit();
                bmp.Freeze();

                ResultImage.Source = bmp;

                // ��ʾ�ߴ���ֽڴ�С
                ImageDimensionsText.Text = $"{bmp.PixelWidth} x {bmp.PixelHeight}";
                ImageSizeText.Text = FormatBytes(bytes.Length);
            }
            catch (FormatException)
            {
                MessageBox.Show("Base64 ����ʧ�ܣ����벻����Ч�� Base64 �ַ�����", "����", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("����ʧ��: " + ex.Message, "����", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (unit < 0) unit = 0; // safety
            return $"{value:0.##} {units[unit]}";
        }
    }
}