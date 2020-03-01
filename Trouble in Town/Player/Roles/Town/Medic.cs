using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trouble_in_Town.Player.Roles.Town
{
    class Medic : Role
    {
        public Medic()
        {
            Name = "Medic";
            Description = "A professional medic! Each night you can choose a player to heal! They will not die that night!";
            AbilityText = "Type the ID of the player to heal them";
            Group = Group.Town;
        }

        protected override void _ability(string ID)
        {
            RunningWindows.GameScreen.Chat($"Healing {ID}-{_playerClients[Convert.ToInt32(ID) - 1].Name}.");
            _client.Send($"{_client._playerClient.ID.ToString()} {(int)Abilities.Heal} {ID}", false);
        }
    }
}
