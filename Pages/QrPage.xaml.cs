using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;

namespace development_kits.Pages
{
    public partial class QrPage : Page
    {
        private readonly ObservableCollection<QrLogEntry> _logs = new();

        public QrPage()
        {
            InitializeComponent();
            LogsList.ItemsSource = _logs;
            Unloaded += QrPage_Unloaded;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            var text = InputText.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("请输入要编码的文本。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var writer = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new EncodingOptions
                    {
                        Height = 300,
                        Width = 300,
                        Margin = 1
                    }
                };

                var pixelData = writer.Write(text);

                using var bmp = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                var bitmapData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);
                try
                {
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bmp.UnlockBits(bitmapData);
                }

                var storedBmp = new Bitmap(bmp);
                var imageSource = BitmapToImageSource(storedBmp);

                var entry = new QrLogEntry
                {
                    Bitmap = storedBmp,
                    Image = imageSource,
                    Text = text,
                    Timestamp = DateTime.Now,
                    IsImageVisible = true
                };

                entry.TimestampString = entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                _logs.Insert(0, entry);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"生成二维码时出错：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveLog_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.DataContext is not QrLogEntry entry) return;

            if (entry.Bitmap == null)
            {
                MessageBox.Show("日志无可保存的图像。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PNG 文件 (*.png)|*.png|JPEG 文件 (*.jpg;*.jpeg)|*.jpg;*.jpeg|Bitmap (*.bmp)|*.bmp",
                FileName = "qrcode.png"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var ext = Path.GetExtension(dlg.FileName).ToLowerInvariant();
                    ImageFormat fmt = ImageFormat.Png;
                    if (ext == ".jpg" || ext == ".jpeg") fmt = ImageFormat.Jpeg;
                    else if (ext == ".bmp") fmt = ImageFormat.Bmp;

                    entry.Bitmap.Save(dlg.FileName, fmt);
                    MessageBox.Show("保存成功。", "完成", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"保存时出错：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ToggleVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.DataContext is not QrLogEntry entry) return;
            entry.IsImageVisible = !entry.IsImageVisible;
        }

        private void ShowOnly_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.DataContext is not QrLogEntry entry) return;
            foreach (var l in _logs) l.IsImageVisible = l == entry;
        }

        private void DeleteLog_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.DataContext is not QrLogEntry entry) return;
            if (_logs.Contains(entry)) { _logs.Remove(entry); try { entry.Bitmap?.Dispose(); } catch { } }
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var l in _logs) { try { l.Bitmap?.Dispose(); } catch { } }
            _logs.Clear();
        }

        private BitmapSource BitmapToImageSource(Bitmap bitmap)
        {
            var handle = bitmap.GetHbitmap();
            try
            {
                var source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    handle,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                source.Freeze();
                return source;
            }
            finally
            {
                NativeMethods.DeleteObject(handle);
            }
        }

        private void QrPage_Unloaded(object? sender, RoutedEventArgs e)
        {
            foreach (var l in _logs) { try { l.Bitmap?.Dispose(); } catch { } }
            _logs.Clear();
        }
    }

    internal class QrLogEntry : INotifyPropertyChanged
    {
        private Bitmap? _bitmap;
        private BitmapSource? _image;
        private string? _text;
        private DateTime _timestamp;
        private string? _timestampString;
        private bool _isImageVisible;

        public Bitmap? Bitmap { get => _bitmap; set { _bitmap = value; OnPropertyChanged(nameof(Bitmap)); } }
        public BitmapSource? Image { get => _image; set { _image = value; OnPropertyChanged(nameof(Image)); } }
        public string? Text { get => _text; set { _text = value; OnPropertyChanged(nameof(Text)); } }
        public DateTime Timestamp { get => _timestamp; set { _timestamp = value; OnPropertyChanged(nameof(Timestamp)); } }
        public string? TimestampString { get => _timestampString; set { _timestampString = value; OnPropertyChanged(nameof(TimestampString)); } }
        public bool IsImageVisible { get => _isImageVisible; set { _isImageVisible = value; OnPropertyChanged(nameof(IsImageVisible)); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}