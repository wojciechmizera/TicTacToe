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
using ShapeControls;

//TODO implement different shapes
//TODO change cursor color for each player


namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int boardSize = 60;
        private int cellSize = 30;


        public MainWindow()
        {
            InitializeComponent();

            // Add rows and columns to the grid
            for (int i = 0; i < boardSize; i++)
            {
                RowDefinition row = new RowDefinition { Height = new GridLength(cellSize) };
                myGrid.RowDefinitions.Add(row);
                ColumnDefinition column = new ColumnDefinition { Width = new GridLength(cellSize) };
                myGrid.ColumnDefinitions.Add(column);
            }

            // Start in tne middle
            scrollViewer.ScrollToVerticalOffset((cellSize * boardSize - Height) / 2);
            scrollViewer.ScrollToHorizontalOffset((cellSize * boardSize - Width) / 2);

        }



        private void normalGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Determine the cell we clicked
            Point position = e.GetPosition(myGrid);
            int row = (int)position.Y / cellSize;
            int col = (int)position.X / cellSize;


            // Avoid overwriting controls
            UserControl control = myGrid.Children.Cast<UserControl>().FirstOrDefault(ctrl => Grid.GetRow(ctrl) == row && Grid.GetColumn(ctrl) == col);
            if (control != null) return;

            // Insert new shape
            XShape shape = new XShape();
            myGrid.Children.Add(shape);
            Grid.SetRow(shape, row);
            Grid.SetColumn(shape, col);

            // TODO: implement
            int maxLength = 0;
            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                int length = 1;
                length = CheckForWinning(row, col, dir);
                if (length > maxLength) maxLength = length;
            }

            if (maxLength >= 5)
            {
                MessageBox.Show($"Player won");
                Title = maxLength.ToString();
                // wtf??
                myGrid.MouseLeftButtonUp -= normalGrid_MouseLeftButtonUp;
            }
        }



        // TODO: fix this shit, apply different pictuure items
        int CheckForWinning(int row, int col, Direction direction)
        {
            int length = 0;

            // Go to the last matching element of specified kind in specified direction
                while (true)
                {
                    switch (direction)
                    {
                        case Direction.Vertical: row--; break;
                        case Direction.Horizontal: col--; break;
                        case Direction.LeftAslant: row--; col--; break;
                        case Direction.RightAslant: row--; col++; break;
                    }
                    UserControl element = myGrid.Children.Cast<UserControl>().FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);

                    // TODO if element is not currentPlayer
                    if (!(element is XShape)) break;
                }

            // Now go back untill first unmatching element and count steps
                while (true)
                {
                    switch (direction)
                    {
                        case Direction.Vertical: row++; break;
                        case Direction.Horizontal: col++; break;
                        case Direction.LeftAslant: row++; col++; break;
                        case Direction.RightAslant: row++; col--; break;
                    }
                    UserControl element = myGrid.Children.Cast<UserControl>().FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);

                if (!(element is XShape))
                    break;
                    length++;
                }

            return length;
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
    }
}
