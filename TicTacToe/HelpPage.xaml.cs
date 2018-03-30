using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for HelpPage.xaml
    /// </summary>
    public partial class HelpPage : Page
    {
        MainWindow hostWindow;

        public HelpPage(MainWindow window)
        {
            InitializeComponent();
            hostWindow = window;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            hostWindow.mainFrame.Content = hostWindow.Game;
        }
    }
}
