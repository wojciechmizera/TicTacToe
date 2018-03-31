using ShapeControls;
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
                new Player(typeof(XShape), "Cross", "GreenArrow.cur"),
                new Player(typeof(OShape), "Circle", "RedArrow.cur"),
                new Player(typeof(SquareShape), "Square", "BlueArrow.cur"),
                new Player(typeof(TriangleShape), "Triangle", "YellowArrow.cur"));

            InitializeGrid();
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

            if (PlayerWon(gridPosition))
            {
                GameOver = true;
            }
            else
            {
                Game.NextPlayer();
                hostWindow.OverrideCursor(Game.CurrentPlayer.Cursor);
            }
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


        bool PlayerWon(System.Drawing.Point position)
        {
            int maxScore = 0;
            Direction maxDirection = Direction.Horizontal;

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

                if (directionScore > maxScore)
                {
                    maxScore = directionScore;
                    maxDirection = direction;
                }
            }
            if (maxScore < Game.WinningScore) return false;

            PaintWinner(maxDirection, position);

            return true;
        }

        private void PaintWinner(Direction direction, System.Drawing.Point position)
        {
            System.Drawing.Point pointForward = new System.Drawing.Point(position.X, position.Y);
            System.Drawing.Point pointBackward = new System.Drawing.Point(position.X, position.Y);

            bool wentForward;
            bool wentBackward;

            CheckCell(position).Background = GetWinningColor();

            do
            {
                MovePoints(direction, ref pointForward, ref pointBackward);

                wentForward = CheckCell(pointForward) != null && CheckCell(pointForward).GetType() == Game.CurrentPlayer.ControlType;
                wentBackward = CheckCell(pointBackward) != null && CheckCell(pointBackward).GetType() == Game.CurrentPlayer.ControlType;

                if (wentForward)
                    CheckCell(pointForward).Background = GetWinningColor();
                

                if (wentBackward)
                    CheckCell(pointBackward).Background = GetWinningColor();

            } while (wentForward || wentBackward);
        }

        private System.Windows.Media.Brush GetWinningColor()
        {
            ResourceDictionary colors = new ResourceDictionary();
            colors.Source = new Uri("Styles/Colors.xaml", UriKind.Relative);
            System.Windows.Media.Brush b = (System.Windows.Media.Brush)colors["WinningControlBrush"];
            return b;
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


    }
}
