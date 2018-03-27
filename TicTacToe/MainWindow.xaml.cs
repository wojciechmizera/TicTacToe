using System;
using System.Collections.Generic;
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
using System.Xml.Serialization;
using ShapeControls;

//TODO lets say 4 players
//TODO fix menu colors, template ??
// TODO options : nr of players, shape for player, nr of shapes in a row to win

// DONE : added saving and loading game
// DONE : Improved checking the score
// DONE : implemented custom controls for shapes
// DONE : change cursor for players
namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int BoardSize { get; set; } = 60;
        public int CellSize { get; set; } = 40;
        public int WinningScore { get; set; } = 5;
        public bool GameOver { get; set; } = false;


        Player currentPlayer;
        int currentPlayerIndex;
        public List<Player> playerList = new List<Player>();




        #region Default constructor
        public MainWindow()
        {
            InitializeComponent();

            // Add rows and columns to the grid
            for (int i = 0; i < BoardSize; i++)
            {
                RowDefinition row = new RowDefinition { Height = new GridLength(CellSize) };
                myGrid.RowDefinitions.Add(row);
                ColumnDefinition column = new ColumnDefinition { Width = new GridLength(CellSize) };
                myGrid.ColumnDefinitions.Add(column);
            }

            // Start in tne middle
            scrollViewer.ScrollToVerticalOffset((CellSize * BoardSize - Height) / 2);
            scrollViewer.ScrollToHorizontalOffset((CellSize * BoardSize - Width) / 2);

            // Create list of players
            playerList.Add(new Player(typeof(XShape), "X", @"C:\Users\Sir\source\repos\GreenArrow.cur"));
            playerList.Add(new Player(typeof(OShape), "O", @"C:\Users\Sir\source\repos\RedArrow.cur"));

            // Set first player
            currentPlayer = playerList[0];
            currentPlayerIndex = 0;
            this.Cursor = new Cursor(currentPlayer.PlayerCursor);

        }
        #endregion


        private void normalGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Determine the cell we clicked
            Point position = e.GetPosition(myGrid);
            int row = (int)position.Y / CellSize;
            int col = (int)position.X / CellSize;

            // Avoid overwriting controls
            UserControl control = myGrid.Children.Cast<UserControl>().FirstOrDefault(ctrl => Grid.GetRow(ctrl) == row && Grid.GetColumn(ctrl) == col);
            if (control != null) return;

            // Generate control and add it to the grid
            UserControl userControl = (UserControl)Activator.CreateInstance(currentPlayer.controlType);
            myGrid.Children.Add(userControl);
            Grid.SetRow(userControl, row);
            Grid.SetColumn(userControl, col);

            currentPlayer.Coordinates.Add(new Coords(row, col));

            int playerScore = CheckScore(row, col);

            if (playerScore >= 5)
            {
                GameEnded();
            }

            // next player
            currentPlayerIndex = (currentPlayerIndex + 1) % playerList.Count;
            currentPlayer = playerList[currentPlayerIndex];
            this.Cursor = new Cursor(currentPlayer.PlayerCursor);
        }

        private void GameEnded()
        {
            MessageBox.Show($"Player {currentPlayer.Description} won");
            myGrid.MouseLeftButtonUp -= normalGrid_MouseLeftButtonUp;
            GameOver = true;
        }




        /// <summary>
        /// Function checking how many shapes in a row we have for every direction
        /// </summary>
        /// <param name="currentRow"></param>
        /// <param name="currentColumn"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        int CheckScore(int row, int column)
        {
            int maxScore = 0;

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                int rowForward = row;
                int columnForward = column;
                int rowBackward = row;
                int columnBackward = column;

                int directionScore = 1;

                // Go to the last matching element of specified kind in specified direction
                UserControl elementForward = null;
                UserControl elementBackward = null;
                do
                {
                    switch (direction)
                    {
                        case Direction.Vertical:
                            rowForward--; rowBackward++;
                            break;
                        case Direction.Horizontal:
                            columnForward--; columnBackward++;
                            break;
                        case Direction.LeftAslant:
                            rowForward--; columnForward--;
                            rowBackward++; columnBackward++;
                            break;
                        case Direction.RightAslant:
                            rowForward--; columnForward++;
                            rowBackward++; columnBackward--;
                            break;
                    }

                    elementForward = myGrid.Children.Cast<UserControl>().FirstOrDefault(e => Grid.GetRow(e) == rowForward && Grid.GetColumn(e) == columnForward);
                    elementBackward = myGrid.Children.Cast<UserControl>().FirstOrDefault(e => Grid.GetRow(e) == rowBackward && Grid.GetColumn(e) == columnBackward);

                    if (elementForward != null && elementForward.GetType() == currentPlayer.controlType) directionScore++;
                    if (elementBackward != null && elementBackward.GetType() == currentPlayer.controlType) directionScore++;

                } while ((elementForward != null && elementForward.GetType() == currentPlayer.controlType) 
                || (elementBackward != null && elementBackward.GetType() == currentPlayer.controlType));

                if (directionScore > maxScore) maxScore = directionScore;
            }
            return maxScore;
        }
        

        #region Managing scrolling


        Point scrollMousePoint = new Point();
        double verticalOffset = 1;
        double horizontalOffset = 1;


        private void scrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            scrollMousePoint = e.GetPosition(scrollViewer);

            verticalOffset = scrollViewer.VerticalOffset;
            horizontalOffset = scrollViewer.HorizontalOffset;

            scrollViewer.CaptureMouse();
        }

        private void scrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (scrollViewer.IsMouseCaptured)
            {
                scrollViewer.ScrollToVerticalOffset(verticalOffset + (scrollMousePoint.Y - e.GetPosition(scrollViewer).Y));
                scrollViewer.ScrollToHorizontalOffset(horizontalOffset + (scrollMousePoint.X - e.GetPosition(scrollViewer).X));
            }

        }

        private void scrollViewer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (verticalOffset != scrollViewer.VerticalOffset || horizontalOffset != scrollViewer.HorizontalOffset)
            {
                e.Handled = true;
            }
            scrollViewer.ReleaseMouseCapture();
        }

        #endregion


        #region Menu Commands

        private void SaveGame_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (myGrid.Children.Count > 0 && !GameOver)
                e.CanExecute = true;
        }

        private void SaveGame_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            using (Stream stream = new FileStream("game.bin", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, playerList);
            }
        }

        private void LoadGame_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void LoadGame_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            myGrid.Children.Clear();
            playerList.Clear();

            try
            {
                using (Stream stream = new FileStream("game.bin", FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    playerList = (List<Player>)formatter.Deserialize(stream);

                    foreach (Player p in playerList)
                    {
                        foreach (Coords c in p.Coordinates)
                        {
                            UserControl userControl = (UserControl)Activator.CreateInstance(p.controlType);
                            myGrid.Children.Add(userControl);
                            Grid.SetRow(userControl, c.GridX);
                            Grid.SetColumn(userControl, c.GridY);
                        }
                    }
                   //TODO fix current player issue
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void NewGame_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        private void NewGame_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            myGrid.Children.Clear();
            foreach (Player player in playerList)
            {
                player.Coordinates = new List<Coords>();
            }
            myGrid.MouseLeftButtonUp += normalGrid_MouseLeftButtonUp;

            GameOver = false;
        }

        #endregion
    }
}
