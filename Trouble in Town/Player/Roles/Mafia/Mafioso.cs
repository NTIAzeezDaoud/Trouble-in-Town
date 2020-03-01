using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trouble_in_Town.ServerClient;

namespace Trouble_in_Town.Player.Roles
{
    public class Mafioso : Role
    {
        public Mafioso()
        {
            Name = "Mafioso";
            Description = "The right hand of the Mafia, every night you can choose one player to kill";
            AbilityText = "Type the ID of the player to kill them";
            Group = Group.Mafia;
        }
        protected override void _ability(string ID)
        {
            RunningWindows.GameScreen.Chat($"Killing {ID}-{_playerClients[Convert.ToInt32(ID) - 1].Name}");
            _client.Send($"{_client._playerClient.ID.ToString()} {(int)Abilities.Kill} {ID}", false);
        }
    }
}
