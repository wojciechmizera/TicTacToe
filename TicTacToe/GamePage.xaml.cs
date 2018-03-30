using ShapeControls;
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

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {

        public bool GameOver { get; set; } = false;

        public GameState Game;

        MainWindow hostWindow;




        public GamePage(MainWindow window)
        {
            InitializeComponent();

            hostWindow = window;


            Game = new GameState(
                new Player(typeof(XShape), "Cross", @"C:\Users\Sir\source\repositories\TicTacToe\TicTacToe\Cursors\GreenArrow.cur"),
                new Player(typeof(OShape), "Circle", @"C:\Users\Sir\source\repositories\TicTacToe\TicTacToe\Cursors\RedArrow.cur"),
                new Player(typeof(SquareShape), "Square", @"C:\Users\Sir\source\repositories\TicTacToe\TicTacToe\Cursors\BlueArrow.cur"),
                new Player(typeof(TriangleShape), "Triangle", @"C:\Users\Sir\source\repositories\TicTacToe\TicTacToe\Cursors\YellowArrow.cur"));

            InitializeGrid();
            hostWindow.Cursor = new Cursor(Game.CurrentPlayer.Cursor);

        }

        public void InitializeGrid()
        {
            for (int i = 0; i < Game.BoardSize; i++)
            {
                RowDefinition row = new RowDefinition { Height = new GridLength(Game.CellSize) };
                myGameGrid.RowDefinitions.Add(row);
                ColumnDefinition column = new ColumnDefinition { Width = new GridLength(Game.CellSize) };
                myGameGrid.ColumnDefinitions.Add(column);
            }

            foreach (Player player in Game)
                foreach (var point in player.Points)
                    AddControlToGrid(player.ControlType, point);


            scrollViewer.ScrollToVerticalOffset((Game.CellSize * Game.BoardSize - hostWindow.Height) / 2);
            scrollViewer.ScrollToHorizontalOffset((Game.CellSize * Game.BoardSize - hostWindow.Width) / 2);
        }

        private void normalGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (GameOver)
                return;

            System.Windows.Point cursorPosition = e.GetPosition(myGameGrid);
            System.Drawing.Point gridPosition = new System.Drawing.Point((int)cursorPosition.X / Game.CellSize, (int)cursorPosition.Y / Game.CellSize);

            if (CheckCell(gridPosition) != null) return;

            AddControlToGrid(Game.CurrentPlayer.ControlType, gridPosition);
            Game.CurrentPlayer.Points.Add(gridPosition);

            int playerScore = CheckScore(gridPosition);
            if (playerScore >= Game.WinningScore)
            {
                GameEnded();
            }

            Game.NextPlayer();
            hostWindow.Cursor = new Cursor(Game.CurrentPlayer.Cursor);
        }

        private void AddControlToGrid(Type controlType, System.Drawing.Point position)
        {
            UserControl control = (UserControl)Activator.CreateInstance(controlType);
            myGameGrid.Children.Add(control);
            Grid.SetRow(control, position.Y);
            Grid.SetColumn(control, position.X);
        }

        private UserControl CheckCell(System.Drawing.Point position)
        {
            return myGameGrid.Children.Cast<UserControl>().FirstOrDefault(ctrl => Grid.GetRow(ctrl) == position.Y && Grid.GetColumn(ctrl) == position.X);
        }



        private void GameEnded()
        {
            MessageBox.Show($"Player {Game.CurrentPlayer.Description} won");
            GameOver = true;
        }

        int CheckScore(System.Drawing.Point position)
        {
            int maxScore = 0;

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                System.Drawing.Point pointForward = new System.Drawing.Point(position.X, position.Y);
                System.Drawing.Point pointBackward = new System.Drawing.Point(position.X, position.Y);

                bool wentForward;
                bool wentBackward;

                int directionScore = 1;
                do
                {
                    MovePoints(direction, ref pointForward, ref pointBackward);

                    wentForward = CheckCell(pointForward) != null && CheckCell(pointForward).GetType() == Game.CurrentPlayer.ControlType;
                    wentBackward = CheckCell(pointBackward) != null && CheckCell(pointBackward).GetType() == Game.CurrentPlayer.ControlType;

                    directionScore += Convert.ToInt32(wentForward) + Convert.ToInt32(wentBackward);

                } while (wentForward || wentBackward);

                if (directionScore > maxScore) maxScore = directionScore;
            }
            return maxScore;
        }

        private void MovePoints(Direction direction, ref System.Drawing.Point pointForward, ref System.Drawing.Point pointBackward)
        {
            switch (direction)
            {
                case Direction.Vertical:
                    pointForward.Y--; pointBackward.Y++;
                    break;
                case Direction.Horizontal:
                    pointForward.X--; pointBackward.X++;
                    break;
                case Direction.LeftAslant:
                    pointForward.Y--; pointForward.X--;
                    pointBackward.Y++; pointBackward.X++;
                    break;
                case Direction.RightAslant:
                    pointForward.Y--; pointForward.X++;
                    pointBackward.Y++; pointBackward.X--;
                    break;
            }
        }

        #region Managing scrolling

        System.Windows.Point scrollMousePoint = new System.Windows.Point();
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
            if (myGameGrid.Children.Count > 0 && !GameOver)
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
                myGameGrid.ColumnDefinitions.RemoveRange(0, Game.BoardSize);
                myGameGrid.RowDefinitions.RemoveRange(0, Game.BoardSize);
                using (Stream stream = new FileStream("game.bin", FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    Game = (GameState)formatter.Deserialize(stream);

                    InitializeGrid();

                    

                    hostWindow.Cursor = new Cursor(Game.CurrentPlayer.Cursor);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void NewGame_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            myGameGrid.Children.Clear();
            foreach (Player player in Game)
                player.Points.Clear();

            GameOver = false;
        }


        private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Options_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            hostWindow.mainFrame.Content = hostWindow.Options;
        }

        private void Help_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            HelpPage page = new HelpPage(hostWindow);
            hostWindow.mainFrame.Content = page;
        }
        #endregion
    }
}
