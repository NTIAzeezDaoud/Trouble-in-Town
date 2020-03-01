using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trouble_in_Town.Player.Roles.Mafia
{
    public class Trickster : Role
    {
        public Trickster()
        {
            Name = "Tricker";
            Description = "A master of trickery in the Mafia. Each night choose a target to change their target to another random target";
            AbilityText = "Type the ID of the player to trick them";
            Group = Group.Mafia;
        }

        protected override void _ability(string ID)
        {
            RunningWindows.GameScreen.Chat($"You have tricked {ID}-{_playerClients[Convert.ToInt32(ID) - 1].Name}, their target has changed!");
            _client.Send($"{_client._playerClient.ID.ToString()} {(int)Abilities.Trick} {ID}", false);
        }
    }
}
