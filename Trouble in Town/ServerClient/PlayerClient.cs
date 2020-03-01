using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Trouble_in_Town.Player;

namespace Trouble_in_Town.ServerClient
{
    public enum PlayerStatus
    {
        Alive, Dead
    }
    public class PlayerClient
    {
        public Socket Socket;
        public Role Role;
        public string Name;
        public string Status;
        public int ID;
        public bool HasVoted;

        public PlayerClient(Socket socket, string name, int ID)
        {
            Socket = socket;
            Name = name;
            Status = "Alive";
            this.ID = ID;
        }

        public void SendVote(string ID)
        {
            int id;
            int.TryParse(ID, out id);

            if (id - 1 == -1 || id > GameplayServer.playerClients.Count)
            {
                RunningWindows.GameScreen.Chat("Invalid Target!");
            }
            else if (GameplayServer.playerClients[id - 1].Status == "Dead")
            {
                RunningWindows.GameScreen.Chat("That player is dead!");
            }
            else if (!HasVoted)
            {
                HasVoted = true;
                RunningWindows.GameScreen.Chat("You have voted for " + id.ToString());
                GameplayServer.Client.Send(id.ToString(), false);
            }
        }
    }
}
