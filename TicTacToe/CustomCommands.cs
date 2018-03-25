using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TicTacToe
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand NewGame = new RoutedUICommand("Starts new game", "NewGameCommand", typeof(CustomCommands));
        public static readonly RoutedUICommand LoadGame = new RoutedUICommand("Loads last saved game", "LoadGameCommand", typeof(CustomCommands));
        public static readonly RoutedUICommand SaveGame = new RoutedUICommand("Saves game", "SaveGameCommand", typeof(CustomCommands));

    }
}
