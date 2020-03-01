using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trouble_in_Town.Player.Roles.Town
{
    class Vigilante : Role
    {
        public Vigilante()
        {
            Name = "Vigilante";
            Description = "Sometimes the laws has flaws and you help the town by killing the mafia. Each night you choose a target to kill.";
            AbilityText = "Type the ID of the player to kill them";
            Group = Group.Town;
        }

        protected override void _ability(string ID)
        {
            RunningWindows.GameScreen.Chat($"You decided to kill {ID}-{_playerClients[Convert.ToInt32(ID) - 1].Name}.");
            _client.Send($"{_client._playerClient.ID.ToString()} {(int)Abilities.Kill} {ID}", false);
        }
    }
}
