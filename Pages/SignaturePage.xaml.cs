using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using DevTools.Helpers;
using DevTools.Resources;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace DevTools.Pages
{
    public partial class SignaturePage : Page
    {
        private BitmapSource? _currentImage;

        public SignaturePage()
        {
            InitializeComponent();
            PenSizeSlider.ValueChanged += PenSizeSlider_ValueChanged;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }

        private void PenSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PenSizeText.Text = e.NewValue.ToString("F0");
            DefaultPenAttributes.Width = e.NewValue;
            DefaultPenAttributes.Height = e.NewValue;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            SignatureCanvas.Strokes.Clear();
            Base64Text.Text = string.Empty;
            _currentImage = null;
            SaveImageButton.Visibility = Visibility.Collapsed;
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            if (SignatureCanvas.Strokes.Count == 0)
            {
                MessageBox.Show(Strings.SignatureEmpty, Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var bitmap = new RenderTargetBitmap(
                    (int)SignatureCanvas.ActualWidth,
                    (int)SignatureCanvas.ActualHeight,
                    96,
                    96,
                    PixelFormats.Pbgra32);

                var visual = new DrawingVisual();
                using (var context = visual.RenderOpen())
                {
                    var brush = new VisualBrush(SignatureCanvas);
                    context.DrawRectangle(brush, null, new Rect(0, 0, SignatureCanvas.ActualWidth, SignatureCanvas.ActualHeight));
                }

                bitmap.Render(visual);
                bitmap.Freeze();

                _currentImage = bitmap;

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                using var stream = new MemoryStream();
                encoder.Save(stream);
                var imageBytes = stream.ToArray();

                Base64Text.Text = Convert.ToBase64String(imageBytes);
                SaveImageButton.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.SignatureGenerateFailed}: {ex.Message}", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CopyBase64_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Base64Text.Text))
            {
                MessageBox.Show(Strings.SignatureEmpty, Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ClipboardHelper.CopyWithFeedback(Base64Text.Text, null);
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentImage == null)
            {
                MessageBox.Show(Strings.SignatureEmpty, Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif|All Files|*.*",
                DefaultExt = ".png",
                FileName = $"signature_{DateTime.Now:yyyyMMdd_HHmmss}.png"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    BitmapEncoder encoder = dialog.FilterIndex switch
                    {
                        2 => new JpegBitmapEncoder(),
                        3 => new BmpBitmapEncoder(),
                        4 => new GifBitmapEncoder(),
                        _ => new PngBitmapEncoder()
                    };

                    encoder.Frames.Add(BitmapFrame.Create(_currentImage));

                    using var stream = new FileStream(dialog.FileName, FileMode.Create);
                    encoder.Save(stream);

                    MessageBox.Show(Strings.ImageSaved, Strings.Success, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{Strings.SaveFailed}: {ex.Message}", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
