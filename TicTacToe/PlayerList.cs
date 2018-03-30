using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    [Serializable]
    public class GameState :IEnumerable
    {
        private int CurrentIndex;
        private List<Player> AllPlayers;
        public Player CurrentPlayer { get; private set; }

        public int NumberOfPlayers { get; set; } = 2;
        public int BoardSize { get; set; } = 60;
        public int CellSize { get; set; } = 40;
        public int WinningScore { get; set; } = 5;


        public GameState(params Player[] players)
        {
            AllPlayers = new List<Player>();
            foreach (Player p in players)
                AllPlayers.Add(p);
            CurrentIndex = 0;
            CurrentPlayer = AllPlayers[CurrentIndex];
        }

        public void Add(Player player)
        {
            AllPlayers.Add(player);
        }

        public void NextPlayer()
        {
            CurrentIndex = (CurrentIndex + 1) % NumberOfPlayers;
            CurrentPlayer = AllPlayers[CurrentIndex];
        }

        public IEnumerator GetEnumerator()
        {
            return AllPlayers.GetEnumerator();
        }
    }
}
