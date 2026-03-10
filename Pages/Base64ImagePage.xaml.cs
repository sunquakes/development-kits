using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using DevTools.Resources;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.MessageBox;
using Application = System.Windows.Application;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using Orientation = System.Windows.Controls.Orientation;
using Image = System.Windows.Controls.Image;
using Cursors = System.Windows.Input.Cursors;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace DevTools.Pages
{
    public partial class Base64ImagePage : Page
    {
        private BitmapSource? _currentImage;
        private byte[]? _currentImageBytes;

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
                MessageBox.Show(Strings.EnterBase64, Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                // Support data URI prefix
                var commaIndex = base64.IndexOf(',');
                if (base64.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && commaIndex >= 0)
                {
                    base64 = base64.Substring(commaIndex + 1);
                }

                var bytes = Convert.FromBase64String(base64);
                _currentImageBytes = bytes;

                using var ms = new MemoryStream(bytes);
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = ms;
                bmp.EndInit();
                bmp.Freeze();

                ResultImage.Source = bmp;
                _currentImage = bmp;

                ImageDimensionsText.Text = $"{bmp.PixelWidth} x {bmp.PixelHeight}";
                ImageSizeText.Text = FormatBytes(bytes.Length);

                // Show action buttons
                SaveButton.Visibility = Visibility.Visible;
                ZoomButton.Visibility = Visibility.Visible;
            }
            catch (FormatException)
            {
                MessageBox.Show(Strings.Base64DecodeFailed, Strings.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
                HideActionButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.DecodeFailed}: {ex.Message}", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                HideActionButtons();
            }
        }

        private void HideActionButtons()
        {
            SaveButton.Visibility = Visibility.Collapsed;
            ZoomButton.Visibility = Visibility.Collapsed;
        }

        private void ResultImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_currentImage != null)
            {
                ShowImagePreview(_currentImage);
            }
        }

        private void ZoomButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentImage != null)
            {
                ShowImagePreview(_currentImage);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentImage == null || _currentImageBytes == null)
            {
                return;
            }

            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "PNG Files|*.png|JPEG Files|*.jpg|BMP Files|*.bmp|All Files|*.*",
                    FileName = $"image_{DateTime.Now:yyyyMMdd_HHmmss}",
                    DefaultExt = ".png"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    SaveImageToFile(_currentImage, saveDialog.FileName);
                    MessageBox.Show($"Image saved to: {saveDialog.FileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.SaveFailed}: {ex.Message}", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowImagePreview(BitmapSource image)
        {
            var previewWindow = new Window
            {
                Title = $"Image Preview - {image.PixelWidth} x {image.PixelHeight}",
                Width = Math.Min(image.PixelWidth + 40, SystemParameters.PrimaryScreenWidth - 100),
                Height = Math.Min(image.PixelHeight + 100, SystemParameters.PrimaryScreenHeight - 100),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black),
                Content = CreatePreviewContent(image)
            };

            // Set owner to main window
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null)
            {
                previewWindow.Owner = mainWindow;
                previewWindow.WindowStyle = WindowStyle.ToolWindow;
            }

            previewWindow.ShowDialog();
        }

        private Grid CreatePreviewContent(BitmapSource image)
        {
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Toolbar
            var toolbar = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            };

            var zoomOutBtn = CreateToolbarButton("&#xf010;", "Zoom Out", () => ZoomImage(grid, 0.9));
            var zoomInBtn = CreateToolbarButton("&#xf00e;", "Zoom In", () => ZoomImage(grid, 1.1));
            var fitBtn = CreateToolbarButton("&#xf065;", "Fit to Window", () => FitImageToWindow(grid, image));
            var saveBtn = CreateToolbarButton("&#xf0c7;", Strings.Save, () => SaveFromPreview(grid, image));
            var closeBtn = CreateToolbarButton("&#xf00d;", "Close", () => {
                var window = grid.Parent as Window;
                window?.Close();
            });

            toolbar.Children.Add(zoomOutBtn);
            toolbar.Children.Add(zoomInBtn);
            toolbar.Children.Add(fitBtn);
            toolbar.Children.Add(saveBtn);
            toolbar.Children.Add(closeBtn);

            // Image with scroll viewer
            var scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent)
            };

            var previewImage = new Image
            {
                Source = image,
                Stretch = Stretch.Uniform,
                Cursor = Cursors.Hand
            };
            previewImage.MouseLeftButtonUp += (s, e) => ZoomImage(grid, 1.2);

            scrollViewer.Content = previewImage;
            grid.Children.Add(scrollViewer);
            Grid.SetRow(scrollViewer, 1);

            // Add toolbar to grid
            grid.Children.Add(toolbar);
            Grid.SetRow(toolbar, 0);

            // Store image reference for later use
            grid.Tag = image;

            return grid;
        }

        private Button CreateToolbarButton(string icon, string toolTip, Action clickAction)
        {
            var button = new Button
            {
                Width = 40,
                Height = 40,
                Margin = new Thickness(5),
                ToolTip = toolTip,
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(60, 60, 60)),
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand
            };

            var textBlock = new TextBlock
            {
                FontFamily = Application.Current.FindResource("FontAwesomeSolid") as System.Windows.Media.FontFamily,
                Text = icon,
                FontSize = 18,
                FontWeight = System.Windows.FontWeights.Bold,
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            button.Content = textBlock;
            button.Click += (s, e) => clickAction();

            return button;
        }

        private void ZoomImage(Grid grid, double zoomFactor)
        {
            if (grid.Children[1] is ScrollViewer scrollViewer && scrollViewer.Content is Image image)
            {
                var currentWidth = image.ActualWidth;
                var newWidth = currentWidth * zoomFactor;
                
                if (newWidth > 50)
                {
                    image.Width = newWidth;
                    image.Stretch = Stretch.None;
                }
            }
        }

        private void FitImageToWindow(Grid grid, BitmapSource image)
        {
            if (grid.Children[1] is ScrollViewer scrollViewer && scrollViewer.Content is Image previewImage)
            {
                previewImage.Width = double.NaN;
                previewImage.Height = double.NaN;
                previewImage.Stretch = Stretch.Uniform;
            }
        }

        private void SaveFromPreview(Grid grid, BitmapSource image)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "PNG Files|*.png|JPEG Files|*.jpg|BMP Files|*.bmp|All Files|*.*",
                    FileName = $"image_{DateTime.Now:yyyyMMdd_HHmmss}",
                    DefaultExt = ".png"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    SaveImageToFile(image, saveDialog.FileName);
                    var window = grid.Parent as Window;
                    MessageBox.Show($"Image saved to: {saveDialog.FileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    window?.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.SaveFailed}: {ex.Message}", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveImageToFile(BitmapSource image, string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            BitmapEncoder encoder = extension switch
            {
                ".jpg" or ".jpeg" => new JpegBitmapEncoder { QualityLevel = 95 },
                ".bmp" => new BmpBitmapEncoder(),
                _ => new PngBitmapEncoder()
            };

            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                encoder.Save(stream);
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
