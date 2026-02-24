using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.Json;

namespace development_kits.Pages
{
    public partial class JsonFormatPage : Page
    {
        private string _lastFormattedJson = string.Empty;

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
            try
            {
                var json = InputText.Text ?? string.Empty;

                // 清空之前生成的可视化内容
                JsonOutputPanel.Children.Clear();

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // 生成格式化后的 JSON 文本（保留大括号等符号），但展示为可折叠视图
                var options = new JsonSerializerOptions { WriteIndented = true };
                var formatted = JsonSerializer.Serialize(root, options);
                _lastFormattedJson = formatted;

                // 根据根节点类型生成可折叠的可视化内容（Expander + StackPanel）
                if (root.ValueKind == JsonValueKind.Object)
                {
                    // 最外层保留大括号
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
                    // 最外层保留中括号
                    var outer = new StackPanel();
                    outer.Children.Add(new TextBlock
                    {
                        Text = "[",
                        FontFamily = new System.Windows.Media.FontFamily("Consolas")
                    });
                    foreach (var el in root.EnumerateArray())
                    {
                        // 数组元素不显示 [0] 前缀
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
                MessageBox.Show("JSON 解析失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private UIElement CreateVisualForElement(string name, JsonElement el)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Object:
                    {
                        // Show opening brace in header like: name: {
                        var headerText = new TextBlock
                        {
                            Text = string.IsNullOrEmpty(name) ? "{" : $"{name}: {{",
                            FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        var exp = new Expander { Header = headerText, IsExpanded = true };
                        var panel = new StackPanel { Margin = new Thickness(12, 4, 0, 4) };
                        foreach (var p in el.EnumerateObject())
                        {
                            panel.Children.Add(CreateVisualForElement(p.Name, p.Value));
                        }

                        // closing brace
                        var closing = new TextBlock
                        {
                            Text = "}",
                            FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                            Margin = new Thickness(4, 2, 0, 2)
                        };
                        panel.Children.Add(closing);

                        exp.Content = panel;
                        return exp;
                    }
                case JsonValueKind.Array:
                    {
                        // Header: if name provided, show "name: [", otherwise just "["
                        var headerText = new TextBlock
                        {
                            Text = string.IsNullOrEmpty(name) ? "[" : $"{name}: [",
                            FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        var exp = new Expander { Header = headerText, IsExpanded = true };
                        var panel = new StackPanel { Margin = new Thickness(12, 4, 0, 4) };
                        foreach (var v in el.EnumerateArray())
                        {
                            // array elements: do not show index prefixes
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
                        return exp;
                    }
                case JsonValueKind.String:
                    {
                        // Use a read-only TextBox so the value can be selected and copied by the user
                        var tb = new TextBox
                        {
                            Text = $"{name}: \"{el.GetString()}\"",
                            IsReadOnly = true,
                            BorderThickness = new Thickness(0),
                            Background = System.Windows.Media.Brushes.Transparent,
                            TextWrapping = TextWrapping.Wrap,
                            Margin = new Thickness(4,2,0,2)
                        };
                        // 双击时，如果光标位于引号内，选中引号内全部内容
                        tb.PreviewMouseDoubleClick += TextBox_PreviewMouseDoubleClick;
                        return tb;
                    }
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    {
                        var val = el.ValueKind == JsonValueKind.Null ? "null" : el.ToString();
                        var tb = new TextBox
                        {
                            Text = $"{name}: {val}",
                            IsReadOnly = true,
                            BorderThickness = new Thickness(0),
                            Background = System.Windows.Media.Brushes.Transparent,
                            Margin = new Thickness(4,2,0,2)
                        };
                        return tb;
                    }
                default:
                    {
                        var val = el.ToString();
                        var tb = new TextBox
                        {
                            Text = $"{name}: {val}",
                            IsReadOnly = true,
                            BorderThickness = new Thickness(0),
                            Background = System.Windows.Media.Brushes.Transparent,
                            Margin = new Thickness(4,2,0,2)
                        };
                        return tb;
                    }
            }
        }

        // Copy helper removed: values are selectable so user can copy by selection.

        private void ExpandAll_Click(object sender, RoutedEventArgs e)
        {
            SetExpanderStateInPanel(JsonOutputPanel, true);
        }

        private void CollapseAll_Click(object sender, RoutedEventArgs e)
        {
            SetExpanderStateInPanel(JsonOutputPanel, false);
        }

        private void SetExpanderStateInPanel(Panel panel, bool expanded)
        {
            foreach (var child in panel.Children)
            {
                if (child is Expander ex)
                {
                    SetExpanderRecursive(ex, expanded);
                }
                else if (child is Panel p)
                {
                    SetExpanderStateInPanel(p, expanded);
                }
            }
        }

        private void SetExpanderRecursive(Expander exp, bool expanded)
        {
            exp.IsExpanded = expanded;
            if (exp.Content is Panel p)
            {
                foreach (var ch in p.Children)
                {
                    if (ch is Expander ce) SetExpanderRecursive(ce, expanded);
                }
            }
        }

        private void CopyFormatted_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_lastFormattedJson))
                {
                    MessageBox.Show("当前没有格式化的 JSON 可复制。", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                Clipboard.SetText(_lastFormattedJson);
            }
            catch
            {
                // ignore clipboard errors
            }
        }

        private void TextBox_PreviewMouseDoubleClick(object? sender, MouseButtonEventArgs e)
        {
            if (sender is not TextBox tb) return;

            // 获取鼠标在文本框中的字符索引
            var pt = e.GetPosition(tb);
            int charIndex = tb.GetCharacterIndexFromPoint(pt, true);
            if (charIndex < 0) return;

            var text = tb.Text ?? string.Empty;

            // 查找左侧最近的双引号
            int left = text.LastIndexOf('"', Math.Min(charIndex, text.Length - 1));
            if (left < 0) return;
            // 查找对应右侧双引号
            int right = text.IndexOf('"', left + 1);
            if (right <= left)
            {
                // 如果没有在左侧之后找到，引号可能在 charIndex 之后
                right = text.IndexOf('"', charIndex);
            }
            if (right <= left) return;

            int start = left + 1;
            int len = right - start;
            if (len > 0)
            {
                tb.Select(start, len);
                e.Handled = true;
            }
        }
    }
}
