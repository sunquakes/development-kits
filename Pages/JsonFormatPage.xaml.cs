using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.Json;
using DevTools.Helpers;
using DevTools.Resources;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;

namespace DevTools.Pages
{
    public partial class JsonFormatPage : Page
    {
        private string _lastFormattedJson = string.Empty;
        private readonly List<Expander> _allExpanders = new List<Expander>();

        public JsonFormatPage()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }

        private void Format_Click(object sender, RoutedEventArgs e)
        {
            var json = InputText.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(json))
            {
                MessageBox.Show(Strings.EnterJSON, Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _allExpanders.Clear();
            JsonOutputPanel.Children.Clear();

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var options = new JsonSerializerOptions { WriteIndented = true };
                var formatted = JsonSerializer.Serialize(root, options);
                _lastFormattedJson = formatted;

                if (root.ValueKind == JsonValueKind.Object)
                {
                    var outer = new StackPanel();
                    outer.Children.Add(new TextBlock
                    {
                        Text = "{",
                        FontFamily = new System.Windows.Media.FontFamily("Consolas")
                    });
                    foreach (var prop in root.EnumerateObject())
                    {
                        outer.Children.Add(CreateVisualForElement(prop.Name, prop.Value));
                    }
                    outer.Children.Add(new TextBlock
                    {
                        Text = "}",
                        FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                        Margin = new Thickness(4, 2, 0, 2)
                    });
                    JsonOutputPanel.Children.Add(outer);
                }
                else if (root.ValueKind == JsonValueKind.Array)
                {
                    var outer = new StackPanel();
                    outer.Children.Add(new TextBlock
                    {
                        Text = "[",
                        FontFamily = new System.Windows.Media.FontFamily("Consolas")
                    });
                    foreach (var el in root.EnumerateArray())
                    {
                        outer.Children.Add(CreateVisualForElement(string.Empty, el));
                    }
                    outer.Children.Add(new TextBlock
                    {
                        Text = "]",
                        FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                        Margin = new Thickness(4, 2, 0, 2)
                    });
                    JsonOutputPanel.Children.Add(outer);
                }
                else
                {
                    JsonOutputPanel.Children.Add(CreateVisualForElement(string.Empty, root));
                }
            }
            catch (JsonException ex)
            {
                MessageBox.Show($"{Strings.JSONFormatFailed}: {ex.Message}", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private UIElement CreateVisualForElement(string name, JsonElement el)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Object:
                    {
                        var arrowIcon = new TextBlock
                        {
                            Text = "▼",
                            FontFamily = new System.Windows.Media.FontFamily("Segoe UI Symbol"),
                            FontSize = 10,
                            Margin = new Thickness(0, 0, 4, 0),
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        var headerText = new TextBlock
                        {
                            Text = string.IsNullOrEmpty(name) ? "{" : $"{name}: {{",
                            FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        var headerPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Children = { arrowIcon, headerText }
                        };

                        var exp = new Expander { Header = headerPanel, IsExpanded = true };
                        
                        var layoutUpdated = false;
                        exp.LayoutUpdated += (s, e) =>
                        {
                            if (!layoutUpdated)
                            {
                                layoutUpdated = true;
                                UpdateArrowIcon(exp, arrowIcon);
                            }
                        };
                        exp.Expanded += (s, e) => UpdateArrowIcon(exp, arrowIcon);
                        exp.Collapsed += (s, e) => UpdateArrowIcon(exp, arrowIcon);
                        
                        var panel = new StackPanel { Margin = new Thickness(12, 4, 0, 4) };
                        foreach (var p in el.EnumerateObject())
                        {
                            panel.Children.Add(CreateVisualForElement(p.Name, p.Value));
                        }

                        var closing = new TextBlock
                        {
                            Text = "}",
                            FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                            Margin = new Thickness(4, 2, 0, 2)
                        };
                        panel.Children.Add(closing);

                        exp.Content = panel;
                        
                        _allExpanders.Add(exp);
                        
                        return exp;
                    }
                case JsonValueKind.Array:
                    {
                        var arrowIcon = new TextBlock
                        {
                            Text = "▼",
                            FontFamily = new System.Windows.Media.FontFamily("Segoe UI Symbol"),
                            FontSize = 10,
                            Margin = new Thickness(0, 0, 4, 0),
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        var headerText = new TextBlock
                        {
                            Text = string.IsNullOrEmpty(name) ? "[" : $"{name}: [",
                            FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        var headerPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Children = { arrowIcon, headerText }
                        };

                        var exp = new Expander { Header = headerPanel, IsExpanded = true };
                        
                        var layoutUpdated = false;
                        exp.LayoutUpdated += (s, e) =>
                        {
                            if (!layoutUpdated)
                            {
                                layoutUpdated = true;
                                UpdateArrowIcon(exp, arrowIcon);
                            }
                        };
                        exp.Expanded += (s, e) => UpdateArrowIcon(exp, arrowIcon);
                        exp.Collapsed += (s, e) => UpdateArrowIcon(exp, arrowIcon);
                        UpdateArrowIcon(exp, arrowIcon);

                        var panel = new StackPanel { Margin = new Thickness(12, 4, 0, 4) };
                        foreach (var v in el.EnumerateArray())
                        {
                            panel.Children.Add(CreateVisualForElement(string.Empty, v));
                        }

                        var closing = new TextBlock
                        {
                            Text = "]",
                            FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                            Margin = new Thickness(4, 2, 0, 2)
                        };
                        panel.Children.Add(closing);

                        exp.Content = panel;
                        
                        _allExpanders.Add(exp);
                        
                        return exp;
                    }
                case JsonValueKind.String:
                    {
                        var text = $"{name}: \"{el.GetString()}\"";
                        var tb = new TextBox
                        {
                            Text = text,
                            IsReadOnly = true,
                            BorderThickness = new Thickness(0),
                            Background = System.Windows.Media.Brushes.Transparent,
                            TextWrapping = TextWrapping.Wrap,
                            Margin = new Thickness(4,2,0,2),
                            FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                            CaretBrush = System.Windows.Media.Brushes.Transparent,
                            SelectionBrush = System.Windows.Media.Brushes.LightBlue,
                            Focusable = true,
                            Template = CreateNoUnderlineTemplate()
                        };
                        tb.MouseDoubleClick += (s, e) =>
                        {
                            var colonIndex = text.IndexOf(':');
                            if (colonIndex >= 0)
                            {
                                var firstQuote = text.IndexOf('"', colonIndex);
                                if (firstQuote >= 0)
                                {
                                    var lastQuote = text.LastIndexOf('"');
                                    if (lastQuote > firstQuote)
                                    {
                                        tb.Select(firstQuote + 1, lastQuote - firstQuote - 1);
                                    }
                                }
                            }
                        };
                        AddContextMenu(tb);
                        return tb;
                    }
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    {
                        var val = el.ValueKind == JsonValueKind.Null ? "null" : el.ToString();
                        var text = $"{name}: {val}";
                        var tb = new TextBox
                        {
                            Text = text,
                            IsReadOnly = true,
                            BorderThickness = new Thickness(0),
                            Background = System.Windows.Media.Brushes.Transparent,
                            Margin = new Thickness(4,2,0,2),
                            FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                            CaretBrush = System.Windows.Media.Brushes.Transparent,
                            SelectionBrush = System.Windows.Media.Brushes.LightBlue,
                            Focusable = true,
                            Template = CreateNoUnderlineTemplate()
                        };
                        tb.MouseDoubleClick += (s, e) =>
                        {
                            var colonIndex = text.IndexOf(':');
                            if (colonIndex >= 0)
                            {
                                var valueStart = colonIndex + 1;
                                while (valueStart < text.Length && char.IsWhiteSpace(text[valueStart]))
                                {
                                    valueStart++;
                                }
                                if (valueStart < text.Length)
                                {
                                    tb.Select(valueStart, text.Length - valueStart);
                                }
                            }
                        };
                        AddContextMenu(tb);
                        return tb;
                    }
                default:
                    {
                        var val = el.ToString();
                        var text = $"{name}: {val}";
                        var tb = new TextBox
                        {
                            Text = text,
                            IsReadOnly = true,
                            BorderThickness = new Thickness(0),
                            Background = System.Windows.Media.Brushes.Transparent,
                            Margin = new Thickness(4,2,0,2),
                            FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                            CaretBrush = System.Windows.Media.Brushes.Transparent,
                            SelectionBrush = System.Windows.Media.Brushes.LightBlue,
                            Focusable = true,
                            Template = CreateNoUnderlineTemplate()
                        };
                        tb.MouseDoubleClick += (s, e) =>
                        {
                            var colonIndex = text.IndexOf(':');
                            if (colonIndex >= 0)
                            {
                                var valueStart = colonIndex + 1;
                                while (valueStart < text.Length && char.IsWhiteSpace(text[valueStart]))
                                {
                                    valueStart++;
                                }
                                if (valueStart < text.Length)
                                {
                                    tb.Select(valueStart, text.Length - valueStart);
                                }
                            }
                        };
                        AddContextMenu(tb);
                        return tb;
                    }
            }
        }

        private static void UpdateArrowIcon(Expander exp, TextBlock arrowIcon)
        {
            arrowIcon.Text = exp.IsExpanded ? "▼" : "▶";
        }

        private static ControlTemplate CreateNoUnderlineTemplate()
        {
            var template = new ControlTemplate(typeof(TextBox));
            var factory = new FrameworkElementFactory(typeof(ScrollViewer));
            factory.Name = "PART_ContentHost";
            template.VisualTree = factory;
            template.Seal();
            return template;
        }

        private void AddContextMenu(TextBox tb)
        {
            var contextMenu = new ContextMenu();
            var copyItem = new MenuItem { Header = Strings.Copy };
            copyItem.Click += (s, e) => ClipboardHelper.CopyWithFeedback(tb.Text, null);
            contextMenu.Items.Add(copyItem);
            tb.ContextMenu = contextMenu;
        }

        private void ExpandAll_Click(object sender, RoutedEventArgs e)
        {
            ExpandOrCollapseRecursive(JsonOutputPanel, true);
            JsonOutputPanel.UpdateLayout();
        }

        private void CollapseAll_Click(object sender, RoutedEventArgs e)
        {
            ExpandOrCollapseRecursive(JsonOutputPanel, false);
            JsonOutputPanel.UpdateLayout();
        }

        private void ExpandOrCollapseRecursive(Panel panel, bool isExpanded)
        {
            foreach (var child in panel.Children)
            {
                if (child is Expander exp)
                {
                    exp.IsExpanded = isExpanded;
                    if (exp.Header is StackPanel headerPanel && headerPanel.Children.Count > 0 && headerPanel.Children[0] is TextBlock arrowIcon)
                    {
                        arrowIcon.Text = isExpanded ? "▼" : "▶";
                    }
                    if (exp.Content is Panel innerPanel)
                    {
                        ExpandOrCollapseRecursive(innerPanel, isExpanded);
                    }
                }
                else if (child is Panel childPanel)
                {
                    ExpandOrCollapseRecursive(childPanel, isExpanded);
                }
            }
        }

        private void CopyFormatted_Click(object sender, RoutedEventArgs e)
        {
            ClipboardHelper.CopyWithFeedback(_lastFormattedJson, (Button)sender);
        }
    }
}
