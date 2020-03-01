using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trouble_in_Town.Player.Roles.Town
{
    public class Investigator : Role
    {
        public Investigator()
        {
            Name = "Investigator";
            Description = "A master detective! Each night you can choose a player to know their role!";
            AbilityText = "Type the ID of the player to investigate them";
            Group = Group.Town;
        }

        protected override void _ability(string ID)
        {
            RunningWindows.GameScreen.Chat($"Investigating {ID}-{_playerClients[Convert.ToInt32(ID) - 1].Name}.");
            _client.Send($"{_client._playerClient.ID.ToString()} {(int)Abilities.Investigate} {ID}", false);
        }
    }
}
