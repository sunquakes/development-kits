using System.Windows;
using System.Windows.Controls;

namespace development_kits.Pages
{
    public partial class QrPage : Page
    {
        public QrPage()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }
    }
}