using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TicTacToe
{
    class Player
    {
        UserControl userShape;
        public Player(UserControl shape)
        {
            userShape = shape;
        }
    }
}
