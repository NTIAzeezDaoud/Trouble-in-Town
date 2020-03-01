using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trouble_in_Town.Player.Roles.Standalone
{
    public class Paranoid : Role
    {
        public Paranoid()
        {
            Name = "Paranoid";
            Description = "You will shoot anyone who visits you because you trust no one! Who knows if you even trust me?";
            AbilityText = "You grasp that cold gun of yours.";
            Group = Group.Standalone;
        }
    }
}
