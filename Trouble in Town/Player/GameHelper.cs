using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Trouble_in_Town.ServerClient;
using Trouble_in_Town.ViewModels;

namespace Trouble_in_Town.Player
{
    public enum Abilities
    {
        Kill, Heal, Investigate, Trick, Bart
    }
    public class AbilityData
    {
        public string From;
        public Abilities Ability;
        public string To;

        public AbilityData(string from, Abilities ability, string to)
        {
            From = from;
            Ability = ability;
            To = to;
        }
    }
    public static class GameHelper
    {
        public static PlayerClient NewPlayerClient(Socket socket, string name) => new PlayerClient(socket, name, GameplayServer.playerClients.Count + 1);

        public static PlayerClient GetPlayer(int ID) => GameplayServer.playerClients[ID - 1];

        public static List<PlayerClient> Mafia = new List<PlayerClient>();

        public static int MafiaAmount;
        public static int TownAmount;
        public static void ChooseRoles()
        {
            MafiaAmount = ChooseMafiaSize();
            int standaloneAmount = (int)Math.Floor((decimal)(MafiaAmount / 2));
            int number;
            string mafia;
            TownAmount = GameplayServer.playerClients.Count - MafiaAmount - standaloneAmount;
            for (int i = 0; i < MafiaAmount; i++)
            {
                RandomUnique(ref GameplayServer.Mafias, ref GameplayServer.Standalones, 1, GameplayServer.playerClients.Count, out number);
                GameplayServer.playerClients[number - 1].Role = RoleHelper.GetRole(RoleHelper.Mafia[i % RoleHelper.Mafia.Length]);
                Mafia.Add(GameplayServer.playerClients[number - 1]);
            }
            for (int i = 0; i < standaloneAmount; i++)
            {
                RandomUnique(ref GameplayServer.Standalones, ref GameplayServer.Mafias, 1, GameplayServer.playerClients.Count, out number);
                GameplayServer.playerClients[number - 1].Role = RoleHelper.GetRole(RoleHelper.Standalone[new Random().Next(0, RoleHelper.Standalone.Length)]);
            }
            for (int i = 0; i < GameplayServer.playerClients.Count - MafiaAmount - standaloneAmount; i++)
            {
                RandomUniqueTown(ref GameplayServer.Towns, ref GameplayServer.Mafias, ref GameplayServer.Standalones, 1, GameplayServer.playerClients.Count, out number);
                GameplayServer.playerClients[number - 1].Role = RoleHelper.GetRole(RoleHelper.Town[i % RoleHelper.Town.Length]);
            }

            mafia = $"The Mafia is: ";

            for (int i = 0; i < Mafia.Count; i++)
            {
                mafia += $"{Mafia[i].ID}-{Mafia[i].Name}";
                if (i == Mafia.Count - 1)
                    break;
                mafia += ", ";
            }

            foreach (PlayerClient playerClient in Mafia)
            {
                GameplayServer.Server.Send(playerClient, $"The Mafia is: {mafia}.");
            }
        }

        public static void RandomUnique(ref List<int> container1, ref List<int> container2, int minValue, int maxValue, out int chosenNumber)
        {
            Random random = new Random();
            int number = random.Next(minValue, maxValue + 1);

            while (container1.Contains(number) || container2.Contains(number))
            {
                number = random.Next(minValue, maxValue + 1);
            }

            container1.Add(number);
            chosenNumber = number;
        }

        public static void RandomUniqueTown(ref List<int> container1, ref List<int> container2, ref List<int> container3, int minValue, int maxValue, out int chosenNumber)
        {
            Random random = new Random();
            int number = random.Next(minValue, maxValue + 1);

            while (container1.Contains(number) || container2.Contains(number) || container3.Contains(number))
            {
                number = random.Next(minValue, maxValue + 1);
            }

            container1.Add(number);
            chosenNumber = number;
        }

        private static int ChooseMafiaSize()
        {
            int players = GameplayServer.playerClients.Count;
            int Mafia = (int)Math.Round(0.5 * Math.Pow(players, 0.72) - 0.37, 0);

            return Mafia;
        }

        public static void SetupPlayerField(string playerDataString, out int thisID)
        {
            string[] data = playerDataString.Split(',');
            bool thisPlayer = false;
            for (int i = 0; i < data.Length; i++)
            {
                thisPlayer = i == data.Length - 1 ? true : false;
                GameScreen.AddPlayer(data[i], "Alive", thisPlayer);
            }
            thisID = data.Length;
        }

        public static string StringifyPlayerField()
        {
            List<string> data = new List<string>();
            for (int i = 0; i < GameplayServer.playerClients.Count; i++)
            {
                data.Add(GameplayServer.playerClients[i].Name);
            }
            return "$ pf " + string.Join(",", data);
        }

        #region PHASES
        public static List<string> Dead = new List<string>();
        public static List<AbilityData> Commands = new List<AbilityData>();

        public static int ToBeLynched = -1;

        public static int PlayerAmount = 0;
        public static void HandleMorning()
        {
            GameEventHandler.HandleNightEvents();

            foreach (string ID in Dead)
            {
                PlayerClient playerClient = GameplayServer.playerClients[Convert.ToInt32(ID) - 1];
                if (playerClient.Role.Group == Group.Mafia)
                    MafiaAmount--;
                else if (playerClient.Role.Group == Group.Town)
                    TownAmount--;
            }

            string deadPlayers = Dead.Count > 0 ? "$ dp " + string.Join(",", Dead) : "$ dp";
            GameplayServer.Server.SendToAll(deadPlayers);

            if (GameEventHandler.ThanatosExists)
                GameEventHandler.CheckForThanatos();

            PlayerAmount -= Dead.Count;

            if (MafiaAmount <= 0)
            {
                GameplayServer.Server.SendToAll("$ vic t");
                GameplayServer.GameOn = false;
            }
            else if (TownAmount <= 0)
            {
                GameplayServer.Server.SendToAll("$ vic m");
                GameplayServer.GameOn = false;
            }
        }

        public static void HandleDiscussion()
        {
            Dead.Clear();
            Commands.Clear();
            GameplayServer.Server.SendToAll("$ di");
        }

        public static void HandleVoting()
        {
            GameEventHandler.BeginVoting();
            GameplayServer.Server.SendToAll("$ vo");
        }

        public static void HandleLynch()
        {
            GameEventHandler.EndVote();
            if (ToBeLynched != -1)
            {
                if (GameplayServer.playerClients[ToBeLynched ].Role.Group == Group.Mafia)
                    MafiaAmount--;
                else if (GameplayServer.playerClients[ToBeLynched].Role.Group == Group.Town)
                    TownAmount--;
                string message = "$ ly " + GameplayServer.playerClients[ToBeLynched].ID;
                message = GameplayServer.playerClients[ToBeLynched].Role.Name == "Fool" ? message + " f" : message;
                GameplayServer.Server.SendToAll(message);
                if (MafiaAmount <= 0)
                {
                    GameplayServer.Server.SendToAll("$ vic t");
                    GameplayServer.GameOn = false;
                }
                else if (TownAmount <= 0)
                {
                    GameplayServer.Server.SendToAll("$ vic m");
                    GameplayServer.GameOn = false;
                }
                ToBeLynched = -1;
            }
            else
            {
                GameplayServer.Server.SendToAll("$ ly");
            }
        }

        public static void HandleNight()
        {
            GameplayServer.Server.SendToAll("$ na");
        }

        #endregion PHASES
    }
}
