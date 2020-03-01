using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trouble_in_Town.Player.Roles;
using Trouble_in_Town.Player.Roles.Mafia;
using Trouble_in_Town.Player.Roles.Town;
using Trouble_in_Town.Player.Roles.Standalone;

namespace Trouble_in_Town.Player
{
    public enum AvailableRoles
    {
        //Mafia
        Mafioso, Bartender, Trickster,

        //Town
        Medic, Investigator, Vigilante,

        //Standalone
        Fool, Thanatos, Paranoid
    }
    public static class RoleHelper
    {
        public static AvailableRoles[] Mafia = { 
            AvailableRoles.Mafioso, 
            AvailableRoles.Bartender, 
            AvailableRoles.Trickster 
        };


        public static AvailableRoles[] Town = { 
            AvailableRoles.Medic, 
            AvailableRoles.Investigator, 
            AvailableRoles.Vigilante 
        };


        public static AvailableRoles[] Standalone = { 
            AvailableRoles.Fool, 
            AvailableRoles.Thanatos, 
            AvailableRoles.Paranoid 
        };
        public static Role GetRole(AvailableRoles role)
        {
            switch (role)
            {
                case AvailableRoles.Mafioso:
                    return new Mafioso();
                case AvailableRoles.Bartender:
                    return new Bartender();
                case AvailableRoles.Trickster:
                    return new Trickster();


                case AvailableRoles.Medic:
                    return new Medic();
                case AvailableRoles.Investigator:
                    return new Investigator();
                case AvailableRoles.Vigilante:
                    return new Vigilante();


                case AvailableRoles.Fool:
                    return new Fool();
                case AvailableRoles.Thanatos:
                    return new Thanatos();
                case AvailableRoles.Paranoid:
                    return new Paranoid();
                default:
                    return null;
            }
        }
    }
}
