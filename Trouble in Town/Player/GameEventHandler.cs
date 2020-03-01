using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trouble_in_Town.ServerClient;

namespace Trouble_in_Town.Player
{
    public static class GameEventHandler
    {
        public static bool ThanatosExists = false;
        public static bool ParanoidExists = false;

        #region NIGHT
        private static void TricksterHandler()
        {
            for (int i = 0; i < GameHelper.Commands.Count; i++)
            {
                if (GameHelper.Commands[i].Ability == Abilities.Trick)
                {
                    for (int j = 0; j > GameHelper.Commands.Count; j++)
                    {
                        if (GameHelper.Commands[i].To == GameHelper.Commands[j].From)
                        {
                            Random r = new Random();
                            GameHelper.Commands[j].To = r.Next(1, GameplayServer.playerClients.Count + 1).ToString();
                            break;
                        }
                    }
                }
            }
            ((Views.ServerTerminal)RunningWindows.GetWindow("ServerTerminal")).Terminal("trick done");
        }

        private static void BartenderHandler()
        {
            for (int i = 0; i < GameHelper.Commands.Count; i++)
            {
                if (GameHelper.Commands[i].Ability == Abilities.Bart)
                {
                    for (int j = 0; j > GameHelper.Commands.Count; j++)
                    {
                        if (GameHelper.Commands[i].To == GameHelper.Commands[j].From)
                        {
                            int drunk = Convert.ToInt32(GameHelper.Commands[j].From);
                            GameHelper.Commands.RemoveAt(j);
                            GameplayServer.Server.Send(GameplayServer.playerClients[drunk - 1], "You were drunk last night");
                            break;
                        }
                    }
                }
            }
            ((Views.ServerTerminal)RunningWindows.GetWindow("ServerTerminal")).Terminal("bart done");
        }

        private static void HealHandler()
        {
            List<int> remove = new List<int>();
            for (int i = 0; i < GameHelper.Commands.Count; i++)
            {
                if (GameHelper.Commands[i].Ability == Abilities.Heal)
                {
                    for (int j = 0; j < GameHelper.Commands.Count; j++)
                    {
                        if (GameHelper.Commands[i].To == GameHelper.Commands[j].To && GameHelper.Commands[j].Ability == Abilities.Kill)
                        {
                            remove.Add(j);
                            
                        }
                    }
                }
            }
            HandleRemove(remove);
            ((Views.ServerTerminal)RunningWindows.GetWindow("ServerTerminal")).Terminal("heal done");
        }

        private static void KillHandler()
        {
            for (int i = 0; i < GameHelper.Commands.Count; i++)
            {
                if (GameHelper.Commands[i].Ability == Abilities.Kill)
                {
                    int killed = Convert.ToInt32(GameHelper.Commands[i].To);
                    GameHelper.Dead.Add(GameHelper.Commands[i].To);
                    GameplayServer.Server.Send(GameplayServer.playerClients[killed - 1], "YOU DIED!");
                }
            }
            ((Views.ServerTerminal)RunningWindows.GetWindow("ServerTerminal")).Terminal("kill done");
        }

        private static void InvestigateHandler()
        {
            for (int i = 0; i < GameHelper.Commands.Count; i++)
            {
                if (GameHelper.Commands[i].Ability == Abilities.Investigate)
                {
                    int from = Convert.ToInt32(GameHelper.Commands[i].From);
                    int to = Convert.ToInt32(GameHelper.Commands[i].To);
                    PlayerClient playerClientFrom = GameplayServer.playerClients[from - 1];
                    PlayerClient playerClientTo = GameplayServer.playerClients[to - 1];
                    string message = $"Your target is a {playerClientTo.Role.Name}";
                    GameplayServer.Server.Send(playerClientFrom, message);
                }
            }
            ((Views.ServerTerminal)RunningWindows.GetWindow("ServerTerminal")).Terminal("Inves done");
        }

        private static void ParanoidHandler()
        {
            foreach (PlayerClient player in GameplayServer.playerClients)
            {
                if (player.Role.Name == "Paranoid")
                {
                    for (int i = 0; i < GameHelper.Commands.Count; i++)
                    {
                        if (GameHelper.Commands[i].To == player.ID.ToString())
                        {
                            GameHelper.Commands[i].Ability = Abilities.Kill;
                            string temp = GameHelper.Commands[i].From;
                            GameHelper.Commands[i].From = GameHelper.Commands[i].To;
                            GameHelper.Commands[i].To = temp;
                        }
                    }
                }
            }
        }

        private static void HandleRemove(List<int> toRemoveList)
        {
            foreach (int index in toRemoveList)
            {
                GameHelper.Commands.RemoveAt(index);
            }
        }

        public static void CheckForThanatos()
        {
            foreach (PlayerClient player in GameplayServer.playerClients)
            {
                if (player.Role.Name == "Thanatos")
                {
                    foreach (string ID in GameHelper.Dead)
                    {
                        if (((Roles.Standalone.Thanatos)player.Role).TargetID == ID)
                        {
                            GameplayServer.Server.SendToAll($"Thanatos claims the soul! {player.ID}-{player.Name} has reached his goal!");
                        }
                    }
                }
            }
        }

        public static void HandleNightEvents()
        {
            if (ParanoidExists)
                ParanoidHandler();
            TricksterHandler();
            BartenderHandler();
            HealHandler();
            KillHandler();
            InvestigateHandler();
        }
        #endregion NIGHT

        #region VOTING

        static int[] Votes = null;
        
        public static void BeginVoting()
        {
            Votes = new int[GameplayServer.playerClients.Count];
        }

        public static void NewVote(string vote)
        {
            int player = Convert.ToInt32(vote) - 1;
            Votes[player]++;
        }

        public static void EndVote()
        {
            int index = -1;
            for (int i = 0; i < Votes.Length; i++)
            {
                if (Votes[i] > (float)GameHelper.PlayerAmount/2)
                {
                    index = i;
                    break;
                }
            }
            GameHelper.ToBeLynched = index;
        }
        #endregion
    }
}
