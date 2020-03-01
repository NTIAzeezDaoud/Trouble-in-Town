using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Trouble_in_Town.Player;
using Trouble_in_Town.ServerClient;

namespace Trouble_in_Town.ViewModels
{
    class GameScreen
    {
        public static void AddPlayer(string name, string status, bool thisPlayer = false)
        {
            RunningWindows.GameScreen.Dispatcher.Invoke(() => {
                Grid playersField = RunningWindows.GameScreen.playersField;
                playersField.RowDefinitions.Add(new RowDefinition());
                int rowCount = playersField.RowDefinitions.Count;

                Label ID = new Label();
                ID.FontWeight = thisPlayer ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal;
                ID.Content = rowCount - 1;
                Grid.SetRow(ID, rowCount - 1);
                Grid.SetColumn(ID, 0);

                Label playerName = new Label();
                playerName.Content = name;
                playerName.FontWeight = thisPlayer ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal;
                Grid.SetRow(playerName, rowCount - 1);
                Grid.SetColumn(playerName, 1);

                Label playerStatus = new Label();
                playerStatus.Content = status;
                playerStatus.FontWeight = thisPlayer ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal;
                playerStatus.Name = "status_" + ID.Content;
                Grid.SetRow(playerStatus, rowCount - 1);
                Grid.SetColumn(playerStatus, 2);

                playersField.Children.Add(ID);
                playersField.Children.Add(playerName);
                playersField.Children.Add(playerStatus);

                GameplayServer.playerClients.Add(new PlayerClient(null, name, Convert.ToInt32(ID.Content)));
            });
        }

        public static void ChangeStatus(string id, string newStatus)
        {
            RunningWindows.GameScreen.Dispatcher.Invoke(() => {
                Grid playersField = RunningWindows.GameScreen.playersField;
                for (int i = 0; i < playersField.Children.Count; i++)
                {
                    Label playerStatus = playersField.Children[i] as Label;
                    if (playerStatus.Name == "status_" + id)
                    {
                        ((Label)playersField.Children[i]).Content = newStatus;
                    }
                }
            });
        }

        public static void ChangeColor(string id)
        {
            RunningWindows.GameScreen.Dispatcher.Invoke(() => {
                Grid playersField = RunningWindows.GameScreen.playersField;
                for (int i = 0; i < playersField.Children.Count; i++)
                {
                    Label playerID = playersField.Children[i] as Label;
                    if (playerID.Content.ToString() == id)
                    {
                        playerID.Background = System.Windows.Media.Brushes.Purple;
                        playerID.Foreground = System.Windows.Media.Brushes.White;
                    }
                }
            });
        }
    }
}
