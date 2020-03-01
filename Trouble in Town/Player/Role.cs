using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trouble_in_Town.ServerClient;

namespace Trouble_in_Town.Player
{
    public enum Group
    {
        Mafia, Town, Standalone
    }
    public abstract class Role
    {
        public string Name;
        public string Description;
        public string AbilityText;
        public bool UsedAbility;
        protected List<PlayerClient> _playerClients = GameplayServer.playerClients;
        protected Client _client = GameplayServer.Client;

        public Group Group;
        public Role()
        {
            UsedAbility = false;
        }
        public virtual void Ability(string ID)
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
            else if (!UsedAbility)
            {
                _ability(ID);
                UsedAbility = true;
            }
        }

        protected virtual void _ability(string ID)
        {
        }
    }
}
