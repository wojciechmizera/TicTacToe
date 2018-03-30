using System;
using System.Collections.Generic;
using System.Drawing;
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

        public Type ControlType { get; set; }
        public string Description { get; set; }
        public string Cursor { get; set; }

        /// <summary>
        /// List of coordinates used for saving and loading the game
        /// </summary>
        public List<Point> Points = new List<Point>();

        public Player(Type userControl, string description, string cursorPath)
        {
            ControlType = userControl;
            Description = description;
            Cursor = cursorPath;
        }

        public Player(){ }

    }
}
