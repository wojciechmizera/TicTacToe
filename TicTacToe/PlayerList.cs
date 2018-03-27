using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    [Serializable]
    class PlayerList :IEnumerable
    {
        private int CurrentIndex;
        private List<Player> AllPlayers;
        public Player Current { get; private set; }

        public PlayerList(params Player[] players)
        {
            AllPlayers = new List<Player>();
            foreach (Player p in players)
                AllPlayers.Add(p);
            CurrentIndex = 0;
            Current = AllPlayers[CurrentIndex];
        }

        public void Add(Player player)
        {
            AllPlayers.Add(player);
        }

        public void NextPlayer()
        {
            CurrentIndex = (CurrentIndex + 1) % AllPlayers.Count;
            Current = AllPlayers[CurrentIndex];
        }

        public IEnumerator GetEnumerator()
        {
            return AllPlayers.GetEnumerator();
        }
    }
}
