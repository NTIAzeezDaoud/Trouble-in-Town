using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Trouble_in_Town.ServerClient;

namespace Trouble_in_Town.Views
{
    /// <summary>
    /// Interaction logic for GameScreen.xaml
    /// </summary>
    public partial class GameScreen : UserControl
    {
        Client client;
        public string ClientName;
        public GameScreen()
        {
            InitializeComponent();
            RunningWindows.GameScreen = this;
            Login login = (Login)RunningWindows.GetWindow("Login");
            ClientName = login.nameText.Text;
            RunningWindows.MainWindow.Title = $"{login.nameText.Text} - {login.addressText.Text}:{login.portText.Text}";
            try
            {
                //client = new Client(login.addressText.Text, Convert.ToInt32(login.portText.Text), ClientName);
                client = new Client("192.168.0.109", 13000, "Bob");
            }
            catch (Exception)
            {

            }
        }

        public void Chat(string message)
        {
            Dispatcher.Invoke(() =>
            {
                chatField.Text += message + "\n";
            });
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(chatBox.Text))
            {
                string textMessage = chatBox.Text;
                if (GameplayServer.Phase == GamePhase.Night)
                    GameplayServer.Client._playerClient.Role.Ability(textMessage);

                else if (GameplayServer.Phase == GamePhase.Voting)
                    GameplayServer.Client._playerClient.SendVote(textMessage);

                else
                    client.Send(textMessage);
                chatBox.Text = "";
            }
        }
    }
}
