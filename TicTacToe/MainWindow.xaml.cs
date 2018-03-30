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

// TODO window icon!!!

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public GamePage Game;

        public MainWindow()
        {
            InitializeComponent();
            Game = new GamePage(this);
            mainFrame.Content = Game;

            Cursor = new Cursor(@"C:\Users\Sir\source\repositories\TicTacToe\TicTacToe\Cursors\BaseArrow.cur");
        }



        private void SaveGame_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Game.myGameGrid.Children.Count > 0 && !Game.GameOver)
                e.CanExecute = true;
        }


        private void SaveGame_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            using (Stream stream = new FileStream("game.bin", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, Game);
            }
        }



        public void LoadGame_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NewGame_Executed(sender, e);
            try
            {
                Game.myGameGrid.ColumnDefinitions.RemoveRange(0, Game.Game.BoardSize);
                Game.myGameGrid.RowDefinitions.RemoveRange(0, Game.Game.BoardSize);
                using (Stream stream = new FileStream("game.bin", FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    Game.Game = (GameState)formatter.Deserialize(stream);

                    Game.InitializeGrid();

                    Cursor = new Cursor(Game.Game.CurrentPlayer.Cursor);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void NewGame_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Game.myGameGrid.Children.Clear();
            foreach (Player player in Game.Game)
                player.Points.Clear();

            Game.GameOver = false;

            mainFrame.Content = Game;
        }


        private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Options_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mainFrame.Content = new OptionsPage(this);
        }

        private void Help_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mainFrame.Content = new HelpPage(this);
        }


    }
}
