using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace development_kits.Helpers
{
    public static class ClipboardHelper
    {
        private static readonly object _lock = new();
        private static DateTime _lastCopyTime = DateTime.MinValue;
        private static DispatcherTimer? _toastTimer;
        private static Popup? _currentPopup;

        private static void ShowToast(string message, Button button, bool isError)
        {
            if (button == null) return;

            if (_currentPopup != null)
            {
                _currentPopup.IsOpen = false;
            }
            _currentPopup = null;

            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(12, 6, 12, 6),
                Child = new TextBlock
                {
                    Text = message,
                    Foreground = Brushes.White,
                    FontSize = 12
                }
            };

            var popup = new Popup
            {
                Child = border,
                Placement = PlacementMode.Right,
                PlacementTarget = button,
                HorizontalOffset = 8,
                VerticalOffset = 0,
                AllowsTransparency = true,
                StaysOpen = true
            };

            _currentPopup = popup;
            popup.IsOpen = true;

            if (_toastTimer != null)
            {
                _toastTimer.Stop();
            }
            _toastTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1500)
            };
            _toastTimer.Tick += (s, e) =>
            {
                if (_toastTimer != null)
                {
                    _toastTimer.Stop();
                }
                popup.IsOpen = false;
                _currentPopup = null;
            };
            _toastTimer.Start();
        }

        public static void CopyWithFeedback(string text, Button? button)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                if (button != null)
                {
                    ShowToast(Resources.Strings.CopyEmpty, button, true);
                }
                return;
            }

            lock (_lock)
            {
                var now = DateTime.Now;
                if ((now - _lastCopyTime).TotalMilliseconds < 100)
                {
                    return;
                }
                _lastCopyTime = now;
            }

            try
            {
                Clipboard.SetText(text);
                if (button != null)
                {
                    ShowToast(Resources.Strings.CopySuccess, button, false);
                }
            }
            catch
            {
                if (button != null)
                {
                    ShowToast(Resources.Strings.CopyEmpty, button, true);
                }
            }
        }
    }
}
