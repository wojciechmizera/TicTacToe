using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
using ShapeControls;

//TODO lets say 4 players
// TODO options : nr of players, shape for player, nr of shapes in a row to win
// TODO menu - przesównica :D
// TODO window icon!!!

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public GamePage Game;
        public HelpPage Help;
        public OptionsPage Options;

        public MainWindow()
        {
            InitializeComponent();
            Game = new GamePage(this);
            Help = new HelpPage(this);
            Options = new OptionsPage(this);
            mainFrame.Content = Game;

            DataContext = new WindowDataContext();
        }


    }
}
