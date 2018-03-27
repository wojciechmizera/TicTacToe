using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TicTacToe
{
    [Serializable]
    public class Player 
    {
        /// <summary>
        /// Type of user control
        /// </summary>

        public Type controlType { get; set; }
        public string Description { get; set; }
        public string PlayerCursor { get; set; }

        /// <summary>
        /// List of coordinates used for saving and loading the game
        /// </summary>
        public List<Coords> Coordinates = new List<Coords>();

        public Player(Type userControl, string description, string cursorPath)
        {
            controlType = userControl;
            Description = description;
            PlayerCursor = cursorPath;
        }

        public Player(){ }

    }


    [Serializable]
    /// <summary>
    /// Class representing coordinates of a shape on the main grid
    /// </summary>
    public class Coords
    {
        public int GridX { get; }
        public int GridY { get; }
        public Coords(int X, int Y)
        {
            GridX = X;
            GridY = Y;
        }

        public Coords(){ }
    }
}
