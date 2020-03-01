using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Trouble_in_Town.ServerClient
{
    public class Client
    {
        Socket _clientSocket;
        IPEndPoint _endPoint;
        Views.GameScreen _gameScreen;
        byte[] _buffer = new byte[1024];
        string _clientName;
        public PlayerClient _playerClient;

        public Client(string address, int port, string name)
        {
            _gameScreen = RunningWindows.GameScreen;
            _endPoint = new IPEndPoint(IPAddress.Parse(address), port);
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientSocket.BeginConnect(_endPoint, new AsyncCallback(ConnectCallback), null);
            _clientName = name;
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                _clientSocket.EndConnect(ar);
                _gameScreen.Chat("Connected!");
                GameplayServer.Client = this;
                Send(_clientName, false);
                _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            }
            catch (Exception e)
            {
                _gameScreen.Chat(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            int received = _clientSocket.EndReceive(ar);
            byte[] data = new byte[received];
            Array.Copy(_buffer, data, received);
            string text = Encoding.ASCII.GetString(data);
            ExecuteCommand(text);
            _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
        }

        private void ExecuteCommand(string text)
        {
            if (text[0] == '$')
            {
                string[] command = text.Split(' ');
                switch (command[1])
                {
                    case "pf":
                        int thisID;
                        Player.GameHelper.SetupPlayerField(command[2], out thisID);
                        _playerClient = new PlayerClient(_clientSocket, _clientName, thisID);
                        break;
                    case "np":
                        ViewModels.GameScreen.AddPlayer(command[2], "Alive");
                        break;
                    case "rl":
                        Player.AvailableRoles role = 0;
                        role = (Player.AvailableRoles)Enum.Parse(role.GetType(), command[2]);
                        _playerClient.Role = Player.RoleHelper.GetRole(role);
                        _gameScreen.Dispatcher.Invoke(() => { 
                            _gameScreen.roleName.Content += _playerClient.Role.Name;
                            _gameScreen.desciptionText.Text = _playerClient.Role.Description;
                        });

                        if (_playerClient.Role.Name == "Thanatos")
                        {
                            string target = command[3];
                            ((Player.Roles.Standalone.Thanatos)_playerClient.Role).TargetID = target;
                            _gameScreen.Chat($"Thanatos wants to claim {target}-{GameplayServer.playerClients[Convert.ToInt32(target) - 1].Name}");
                            ViewModels.GameScreen.ChangeColor(command[3]);
                        }
                        break;

                    case "di":
                        _gameScreen.Chat("DAY! The town may now discuss!");
                        GameplayServer.Phase = GamePhase.Discussion;
                        break;

                    case "vo":
                        if (_playerClient.Status == "Alive")
                            _gameScreen.Chat("VOTING! Type the ID of the player to vote.");
                        GameplayServer.Phase = GamePhase.Voting;
                        break;

                    case "ly":
                        _playerClient.HasVoted = false;
                        GameplayServer.Phase = GamePhase.Lynch;
                        if (command.Length > 2)
                        {
                            int id = Convert.ToInt32(command[2]);
                            _gameScreen.Chat($"{id}-{GameplayServer.playerClients[id - 1].Name} was lynched!");
                            ViewModels.GameScreen.ChangeStatus(command[2], "Dead");
                            if (command.Length == 4)
                            {
                                switch (command[3])
                                {
                                    case "f":
                                        _gameScreen.Chat("Uh...Oh...The one you just lynched was the FOOL! You made him reach his goal... to die!");
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        break;

                    case "na":
                        GameplayServer.Phase = GamePhase.Night;
                        _gameScreen.Chat("NIGHT! The skies darken as the sun sets.");
                        if (_playerClient.Status == "Alive")
                            _gameScreen.Chat(_playerClient.Role.AbilityText);
                        break;

                    case "dp":
                        _playerClient.Role.UsedAbility = false;
                        GameplayServer.Phase = GamePhase.Discussion;
                        string dead = "";
                        string[] deadPlayers = { };
                        
                        if (command.Length >= 3)
                        {
                            deadPlayers = command[2].Split(',');
                            
                            foreach (string id in deadPlayers)
                            {
                                int ID = Convert.ToInt32(id);
                                PlayerClient playerClient = GameplayServer.playerClients[ID - 1];
                                GameplayServer.playerClients[ID - 1].Status = "Dead";
                                dead += $"{playerClient.ID}-{playerClient.Name} ";
                            }
                            _gameScreen.Chat("MORNING! " + dead + "died last night.");
                        }
                        else
                        {
                            _gameScreen.Chat("MORNING! No one died last night!");
                        }
                        
                        foreach (string id in deadPlayers)
                        {
                            ViewModels.GameScreen.ChangeStatus(id, "Dead");
                        }
                        break;

                    case "vic":
                        if (command[2] == "m")
                            _gameScreen.Chat("The Mafia wins!");
                        else if (command[2] == "t")
                            _gameScreen.Chat("The Town wins!");
                        break;

                    default:
                        break;
                }
            }
            else
                _gameScreen.Chat(text);
        }

        public void Send(string message, bool chat = true)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);

            if(!chat)
                _clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), message);
            else
                _clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallbackChat), message);
        }

        private void SendCallbackChat(IAsyncResult ar)
        {
            if (_playerClient.Status == "Alive")
            {
                _clientSocket.EndSend(ar);
                _gameScreen.Chat("You: " + (string)ar.AsyncState);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            _clientSocket.EndSend(ar);
        }
    }
}
