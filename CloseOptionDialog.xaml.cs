using System.Windows;
using DevTools.Resources;

namespace DevTools
{
    public partial class CloseOptionDialog : Window
    {
        public bool MinimizeToTray { get; private set; } = true;

        public CloseOptionDialog()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            MinimizeToTray = MinimizeToTrayRadio.IsChecked == true;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
