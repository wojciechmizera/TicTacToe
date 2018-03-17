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

        /// <summary>
        /// List of images representing current players' <see cref="currentPlayer"/> images
        /// </summary>
        private List<BitmapImage> imageList = new List<BitmapImage>();

        /// <summary>
        /// Index of current image in <see cref="imageList"/>
        /// </summary>
        private int currentPlayer = 0;

        public MainWindow()
        {
            InitializeComponent();

            // Add rows and columns to the grid
            for (int i = 0; i < boardSize; i++)
            {
                RowDefinition row = new RowDefinition { Height = new GridLength(30) };
                normalGrid.RowDefinitions.Add(row);
                ColumnDefinition column = new ColumnDefinition { Width = new GridLength(30) };
                normalGrid.ColumnDefinitions.Add(column);
            }

            // Start in tne middle
            scrollViewer.ScrollToVerticalOffset((cellSize * boardSize - Height) / 2);
            scrollViewer.ScrollToHorizontalOffset((cellSize * boardSize - Width) / 2);

            imageList.Add(new BitmapImage(new Uri(@"Shapes\Circle.png",UriKind.Relative)));
            imageList.Add(new BitmapImage(new Uri(@"Shapes\Square.png", UriKind.Relative)));

        }



        private void normalGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(normalGrid);
            int row = (int)position.Y / cellSize;
            int col = (int)position.X / cellSize;

            Image img = new Image();
            img.Margin = new Thickness(1);
            img.Source = imageList[currentPlayer];

            normalGrid.Children.Add(img);
            Grid.SetRow(img, row);
            Grid.SetColumn(img, col);

            // TODO: implement
            int maxLength = 0;
            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                int length = 1;
                length = CheckForWinning(row, col, dir);
                if (length > maxLength) maxLength = length;
            }

            if (maxLength >= 5) MessageBox.Show($"Player {imageList[currentPlayer].BaseUri} won");

            currentPlayer = (currentPlayer + 1) % imageList.Count;
            Title = maxLength.ToString();

        }


        // TODO: fix this shit, apply different pictuure items
        int CheckForWinning(int row, int col, Direction direction)
        {
            int length = 0;

            // Go to the last matching element of specified kind in specified direction
            try
            {
                while (true)
                {
                    switch (direction)
                    {
                        case Direction.Vertical: row--; break;
                        case Direction.Horizontal: col--; break;
                        case Direction.LeftAslant: row--; col--; break;
                        case Direction.RightAslant: row--; col++; break;
                    }
                    UIElement element = normalGrid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);
                    if (((Image)element).Source != imageList[currentPlayer]) throw new Exception();
                }
            }
            catch (Exception) { }

            // Now go back untill first unmatching element and count steps
            try
            {
                while (true)
                {
                    switch (direction)
                    {
                        case Direction.Vertical: row++; break;
                        case Direction.Horizontal: col++; break;
                        case Direction.LeftAslant: row++; col++; break;
                        case Direction.RightAslant: row++; col--; break;
                    }
                    UIElement element = normalGrid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);
                    if (((Image)element).Source != imageList[currentPlayer]) throw new Exception();

                    length++;
                }
            }
            catch (Exception) { }
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
