using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trouble_in_Town.Player.Roles.Standalone
{
    public class Fool : Role
    {
        public Fool()
        {
            Name = "Fool";
            Description = "The clown, the jester... The FOOL! Your only goal is to get the town to lynch you. Why? I do not know!";
            AbilityText = "The FOOL is asleep!";
            Group = Group.Standalone;
        }
    }
}
