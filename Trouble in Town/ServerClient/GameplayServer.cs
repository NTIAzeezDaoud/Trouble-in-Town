using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Timers;
using Trouble_in_Town.Player.Roles;
using Trouble_in_Town.Player;

namespace Trouble_in_Town.ServerClient
{
    public enum GamePhase
    {
        GameStart, Morning, Discussion, Voting, Lynch, Night
    }
    public static class GameplayServer
    {
        public static GamePhase Phase;
        public static List<PlayerClient> playerClients = new List<PlayerClient>();
        public static List<int> Mafias = new List<int>();
        public static List<int> Towns = new List<int>();
        public static List<int> Standalones = new List<int>();

        public static Client Client;

        public static Server Server;

        public static bool GameOn = false;
        public static bool ChatOn = true;
        private static Timer timer;

        public static void BeginGame()
        {
            GameOn = true;
            GameHelper.ChooseRoles();
            GameHelper.PlayerAmount = playerClients.Count;
            timer = new Timer(10000);
            timer.Elapsed += GameStartEnded;
            ((Views.ServerTerminal)RunningWindows.GetWindow("ServerTerminal")).Terminal("Game Started");

            foreach (PlayerClient item in playerClients)
            {
                if (item.Role.Name == "Thanatos")
                {
                    GameEventHandler.ThanatosExists = true;
                    int target = new Random().Next(1, playerClients.Count + 1);
                    ((Player.Roles.Standalone.Thanatos)item.Role).TargetID = target.ToString();
                    Server.Send(item, $"$ rl {item.Role.GetType().Name} {target}");
                }
                else if (item.Role.Name == "Paranoid")
                {
                    GameEventHandler.ParanoidExists = true;
                    Server.Send(item, $"$ rl {item.Role.GetType().Name}");
                }
                else
                    Server.Send(item, $"$ rl {item.Role.GetType().Name}");

            }
            timer.Start();
        }

        private static void GameStartEnded(object sender, ElapsedEventArgs e) // NIGHT START
        {
            Phase = GamePhase.Night;
            timer.Elapsed -= GameStartEnded;
            timer = new Timer(45000);
            timer.Elapsed += MorningStart;
            GameHelper.HandleNight();
            ((Views.ServerTerminal)RunningWindows.GetWindow("ServerTerminal")).Terminal("Night Started");
            timer.Start();
        }

        private static void MorningStart(object sender, ElapsedEventArgs e)
        {
            Phase = GamePhase.Morning;
            GameHelper.HandleMorning();
            ((Views.ServerTerminal)RunningWindows.GetWindow("ServerTerminal")).Terminal("Morning Started");
            if (GameOn)
            {
                timer.Elapsed -= MorningStart;
                timer = new Timer(5000);
                timer.Elapsed += DiscussionStart;
                timer.Start();
            }
            else
            {
                timer.Stop();
                ((Views.ServerTerminal)RunningWindows.GetWindow("ServerTerminal")).Terminal("Game Ended");
            }
            
        }

        private static void DiscussionStart(object sender, ElapsedEventArgs e)
        {
            Phase = GamePhase.Discussion;
            timer.Elapsed -= DiscussionStart;
            timer = new Timer(90000);
            timer.Elapsed += VotingStart;
            GameHelper.HandleDiscussion();
            ((Views.ServerTerminal)RunningWindows.GetWindow("ServerTerminal")).Terminal("Discussion Started");
            timer.Start();
        }

        private static void VotingStart(object sender, ElapsedEventArgs e)
        {
            Phase = GamePhase.Voting;
            timer.Elapsed -= VotingStart;
            timer = new Timer(20000);
            timer.Elapsed += LynchStart;
            GameHelper.HandleVoting();
            ((Views.ServerTerminal)RunningWindows.GetWindow("ServerTerminal")).Terminal("Voting Started");
            timer.Start();
        }

        private static void LynchStart(object sender, ElapsedEventArgs e)
        {
            Phase = GamePhase.Lynch;
            GameHelper.HandleLynch();
            ((Views.ServerTerminal)RunningWindows.GetWindow("ServerTerminal")).Terminal("Lynch Started");
            if (GameOn)
            {
                timer.Elapsed -= LynchStart;
                timer = new Timer(5000);
                timer.Elapsed += NightStart;
                timer.Start();
            }
            else
            {
                timer.Stop();
                ((Views.ServerTerminal)RunningWindows.GetWindow("ServerTerminal")).Terminal("Game Ended");
            }
        }

        private static void NightStart(object sender, ElapsedEventArgs e)
        {
            Phase = GamePhase.Night;
            timer.Elapsed -= NightStart;
            timer = new Timer(45000);
            timer.Elapsed += MorningStart;
            GameHelper.HandleNight();
            ((Views.ServerTerminal)RunningWindows.GetWindow("ServerTerminal")).Terminal("Night Started");
            timer.Start();
        }
    }
}
