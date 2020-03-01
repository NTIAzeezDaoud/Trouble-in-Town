using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trouble_in_Town.Player.Roles
{
    public class Bartender : Role
    {
        public Bartender()
        {
            Name = "Bartender";
            Description = "A part of the Mafia, each night choose a target to serve Alcohol to. They will be unable to use their ability that night.";
            AbilityText = "Type the ID of the player to serve them a drink";
            Group = Group.Mafia;
        }

        protected override void _ability(string ID)
        {
            RunningWindows.GameScreen.Chat($"Giving {ID}-{_playerClients[Convert.ToInt32(ID) - 1].Name} a whole lot of alcohol!");
            _client.Send($"{_client._playerClient.ID.ToString()} {(int)Abilities.Bart} {ID}", false);
        }
    }
}
