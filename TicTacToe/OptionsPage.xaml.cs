using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for OptionsPage.xaml
    /// </summary>
    public partial class OptionsPage : Page
    {
        MainWindow hostWindow;
        GameState Game;
        public OptionsPage(MainWindow window)
        {
            InitializeComponent();
            hostWindow = window;

            Game = hostWindow.Game.Game;

            scrlCellSize.Value = Game.CellSize;
            scrlGridSize.Value = Game.BoardSize;
            scrlPlayers.Value = Game.NumberOfPlayers;
            scrlShapes.Value = Game.WinningScore;

            SetScrollBindings();

        }

        private void SetScrollBindings()
        {
            SetBinding(txtCellSize, scrlCellSize);
            SetBinding(txtGridSize, scrlGridSize);
            SetBinding(txtPlayers, scrlPlayers);
            SetBinding(txtShapes, scrlShapes);
        }

        private void SetBinding(TextBlock textBox, ScrollBar scrollBar)
        {
            Binding binding = new Binding();
            binding.Converter = new DoubleToIntValueConverter();
            binding.Source = scrollBar;
            binding.Path = new PropertyPath("Value");
            textBox.SetBinding(TextBlock.TextProperty, binding);
        }


        #region Menu Commands


        private void LoadGame_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            hostWindow.mainFrame.Content = hostWindow.Game;
            hostWindow.Game.NewGame_Executed(sender, e);
            hostWindow.Game.LoadGame_Executed(sender, e);
        }


        private void NewGame_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            hostWindow.mainFrame.Content = hostWindow.Game;
            hostWindow.Game.NewGame_Executed(sender, e);
        }


        private void CanNotExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }


        private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Help_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            hostWindow.mainFrame.Content = hostWindow.Help;
        }
        #endregion


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            hostWindow.mainFrame.Content = hostWindow.Game;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            hostWindow.Game.myGameGrid.ColumnDefinitions.RemoveRange(0, Game.BoardSize);
            hostWindow.Game.myGameGrid.RowDefinitions.RemoveRange(0, Game.BoardSize);

            UpdateGameSettings();

           hostWindow.Game.myGameGrid.Children.Clear();
            foreach (Player player in Game)
                player.Points.Clear();

            hostWindow.Game.InitializeGrid();

            hostWindow.mainFrame.Content = hostWindow.Game;
        }

        private void UpdateGameSettings()
        {
            hostWindow.Game.Game.CellSize = (int)scrlCellSize.Value;
            hostWindow.Game.Game.BoardSize = (int)scrlGridSize.Value;
            hostWindow.Game.Game.NumberOfPlayers = (int)scrlPlayers.Value;
            hostWindow.Game.Game.WinningScore = (int)scrlShapes.Value;

        }
    }
}

