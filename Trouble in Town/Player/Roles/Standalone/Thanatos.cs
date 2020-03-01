using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trouble_in_Town.Player.Roles.Standalone
{
    class Thanatos : Role
    {
        public string TargetID;
        public Thanatos()
        {
            Name = "Thanatos";
            Description = "A man (or woman) on a mission by the god of death. You win if your target is dead!";
            AbilityText = "Well, there is nothing you can do at night ¯\\_(ツ)_/¯";
            Group = Group.Standalone;
        }
    }
}
