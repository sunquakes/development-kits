using System.Windows;
using System.Windows.Controls;

namespace development_kits.Pages
{
    public partial class BarcodePage : Page
    {
        public BarcodePage()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }
    }
}