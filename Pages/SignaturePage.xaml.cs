using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using Microsoft.Win32;
using DevTools.Helpers;
using DevTools.Resources;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using Cursors = System.Windows.Input.Cursors;
using Orientation = System.Windows.Controls.Orientation;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Image = System.Windows.Controls.Image;
using Color = System.Windows.Media.Color;
using Brushes = System.Windows.Media.Brushes;
using Application = System.Windows.Application;
using FontFamily = System.Windows.Media.FontFamily;
using Point = System.Windows.Point;

namespace DevTools.Pages
{
    public partial class SignaturePage : Page
    {
        private byte[]? _currentImageBytes;
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
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            SignatureCanvas.Strokes.Clear();
            ClearResult();
        }

        private void ClearResult()
        {
            Base64Text.Text = string.Empty;
            ImageDimensionsText.Text = "-";
            ImageSizeText.Text = "-";
            _currentImageBytes = null;
            _currentImage = null;
            ResultImage.Source = null;
            SaveImageButton.Visibility = Visibility.Collapsed;
            ImageBorder.Visibility = Visibility.Collapsed;
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
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
                _currentImageBytes = stream.ToArray();

                var base64 = Convert.ToBase64String(_currentImageBytes);
                Base64Text.Text = base64;

                ImageDimensionsText.Text = $"{bitmap.PixelWidth} x {bitmap.PixelHeight}";
                ImageSizeText.Text = FormatBytes(_currentImageBytes.Length);

                ResultImage.Source = _currentImage;
                SaveImageButton.Visibility = Visibility.Visible;
                ImageBorder.Visibility = Visibility.Visible;
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

            SaveImage(_currentImage);
        }

        private void ImageBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_currentImage == null)
            {
                MessageBox.Show(Strings.NoImageToPreview, Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var previewWindow = new Window
            {
                Title = $"{Strings.ImagePreview} - {_currentImage.PixelWidth} x {_currentImage.PixelHeight}",
                Width = Math.Min(_currentImage.PixelWidth + 40, SystemParameters.WorkArea.Width * 0.9),
                Height = Math.Min(_currentImage.PixelHeight + 100, SystemParameters.WorkArea.Height * 0.9),
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Background = new SolidColorBrush(Color.FromRgb(18, 18, 18))
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            var toolbar = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10)
            };

            var saveButton = CreateIconButton();
            saveButton.Click += (s, args) => SaveImage(_currentImage);
            toolbar.Children.Add(saveButton);

            grid.Children.Add(toolbar);
            Grid.SetRow(toolbar, 0);

            var image = new Image
            {
                Source = _currentImage,
                Stretch = Stretch.Uniform,
                Margin = new Thickness(10)
            };

            grid.Children.Add(image);
            Grid.SetRow(image, 1);

            previewWindow.Content = grid;
            previewWindow.ShowDialog();
        }

        private Button CreateIconButton()
        {
            var textBlock = new TextBlock
            {
                FontFamily = (FontFamily)Application.Current.FindResource("FontAwesomeSolid"),
                Text = "\uf0c7",
                FontSize = 18,
                Foreground = Brushes.White,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            var button = new Button
            {
                Content = textBlock,
                Width = 40,
                Height = 40,
                Cursor = System.Windows.Input.Cursors.Hand,
                Background = new SolidColorBrush(Color.FromRgb(74, 74, 74)),
                BorderThickness = new Thickness(0),
                ToolTip = Strings.SaveImage
            };

            var template = new ControlTemplate(typeof(Button));
            var borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.Name = "border";
            borderFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(20));
            borderFactory.SetValue(Border.SnapsToDevicePixelsProperty, true);

            var contentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            borderFactory.AppendChild(contentFactory);
            template.VisualTree = borderFactory;

            var hoverTrigger = new Trigger { Property = Button.IsMouseOverProperty, Value = true };
            hoverTrigger.Setters.Add(new Setter(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(90, 90, 90)), "border"));
            hoverTrigger.Setters.Add(new Setter(Border.EffectProperty, new DropShadowEffect { Color = Color.FromArgb(102, 0, 0, 0), BlurRadius = 12, ShadowDepth = 2 }, "border"));
            template.Triggers.Add(hoverTrigger);

            var pressedTrigger = new Trigger { Property = Button.IsPressedProperty, Value = true };
            pressedTrigger.Setters.Add(new Setter(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(106, 106, 106)), "border"));
            template.Triggers.Add(pressedTrigger);

            button.Template = template;
            return button;
        }

        private void SaveImage(BitmapSource image)
        {
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

                    encoder.Frames.Add(BitmapFrame.Create(image));

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

        private string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
