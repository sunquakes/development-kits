using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using DevTools.Helpers;
using DevTools.Resources;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using Cursors = System.Windows.Input.Cursors;
using Orientation = System.Windows.Controls.Orientation;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Button = System.Windows.Controls.Button;
using Image = System.Windows.Controls.Image;
using Color = System.Windows.Media.Color;
using Brushes = System.Windows.Media.Brushes;
using Application = System.Windows.Application;
using FontFamily = System.Windows.Media.FontFamily;
using Point = System.Windows.Point;

namespace DevTools.Pages
{
    public partial class Base64ImagePage : Page
    {
        private BitmapSource? _currentImage;

        public Base64ImagePage()
        {
            InitializeComponent();
            LoadState();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            SaveState();
            NavigationService?.Navigate(new HomePage());
        }

        private void SaveState()
        {
            var state = new Dictionary<string, string>
            {
                { "InputText", InputText.Text ?? string.Empty }
            };
            PageStateManager.SavePageState(this, state);
        }

        private void LoadState()
        {
            var state = PageStateManager.GetPageState(this);
            if (state != null)
            {
                InputText.Text = state.GetValueOrDefault("InputText", string.Empty);
            }
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
                _currentImage = bmp;

                ImageDimensionsText.Text = $"{bmp.PixelWidth} x {bmp.PixelHeight}";
                ImageSizeText.Text = FormatBytes(bytes.Length);

                ImageBorder.ToolTip = Strings.ClickToPreview;
                ImageBorder.Cursor = Cursors.Hand;
            }
            catch (FormatException)
            {
                MessageBox.Show(Strings.Base64DecodeFailed, Strings.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.DecodeFailed}: {ex.Message}", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                FileName = $"image_{DateTime.Now:yyyyMMdd_HHmmss}.png"
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

                    using var stream = new FileStream(dialog.FileName, FileMode.Create, FileAccess.Write);
                    encoder.Save(stream);

                    MessageBox.Show(string.Format(Strings.ImageSaved, dialog.FileName), Strings.Success, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{Strings.SaveFailed}: {ex.Message}", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
    }
}
