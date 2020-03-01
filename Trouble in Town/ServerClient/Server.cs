using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Trouble_in_Town.Views;
using Trouble_in_Town.ServerClient;
using Trouble_in_Town.Player;

namespace Trouble_in_Town
{
    public class Server
    {
        private List<PlayerClient> _clients { get => GameplayServer.playerClients; set => value = GameplayServer.playerClients; }
        private byte[] _buffer = new byte[1024];
        private IPEndPoint _ipEndPoint;
        private Socket _serverSocket;
        private ServerTerminal server;
        public Server(string address, int port)
        {
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
            server = (ServerTerminal)RunningWindows.GetWindow("ServerTerminal");
            server.UpdateInformation(_ipEndPoint.Address.ToString(), _ipEndPoint.Port);
            ServerSetup();
        }

        private void ServerSetup()
        {
            server.Terminal("Setting up...");
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(_ipEndPoint);
            server.Terminal("Ready!");
            GameplayServer.Server = this;
            _serverSocket.Listen(1);

            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = _serverSocket.EndAccept(ar);
                server.Terminal("Connected with " + socket.RemoteEndPoint.ToString());
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallbackNew), socket);
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }
            catch (Exception e)
            {
                server.Terminal(e.ToString());
            }
            
        }

        private void ReceiveCallbackNew(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            int received = socket.EndReceive(ar);
            byte[] data = new byte[received];
            Array.Copy(_buffer, data, received);
            string text = Encoding.ASCII.GetString(data);

            PlayerClient playerClient = GameHelper.NewPlayerClient(socket, text);
            _clients.Add(playerClient);
            server.Terminal($"Created Player Client for {_clients[_clients.Count - 1].Name}");
            Send(_clients[_clients.Count - 1], GameHelper.StringifyPlayerField());
            SendToAll("$ np " + _clients[_clients.Count - 1].Name, _clients[_clients.Count - 1]);
            _clients[_clients.Count - 1].Socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), _clients[_clients.Count - 1]);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                PlayerClient playerClient = (PlayerClient)ar.AsyncState;
                
                int received = playerClient.Socket.EndReceive(ar);
                byte[] data = new byte[received];
                Array.Copy(_buffer, data, received);
                string text = Encoding.ASCII.GetString(data);
                server.Terminal(playerClient.Name + ": " + text);

                ExecuteCommand(text, playerClient);
            }
            catch (Exception e)
            {
                server.Terminal(e.ToString());
            }
        }

        public void Send(PlayerClient playerClient, string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);

            playerClient.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), playerClient);
            server.Terminal("Sent: " + Encoding.ASCII.GetString(data));
        }
        public void SendToAll(string message, params PlayerClient[] except)
        {
            foreach (PlayerClient client in _clients)
            {
                if (!except.Contains(client))
                {
                    Send(client, message);
                }
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                PlayerClient playerClient = (PlayerClient)ar.AsyncState;
                playerClient.Socket.EndSend(ar);
                playerClient.Socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), playerClient);
            }
            catch (Exception e)
            {
                server.Terminal(e.ToString());
            }
        }

        private void ExecuteCommand(string command, PlayerClient playerClient)
        {
            string[] splitCommand = command.Split(' ');
            switch (GameplayServer.Phase)
            {
                case GamePhase.GameStart:
                case GamePhase.Discussion:
                    foreach (PlayerClient playerClient1 in _clients)

                        if (playerClient1.Socket == playerClient.Socket)
                            continue;
                        else
                            Send(playerClient1, playerClient.Name + ": " + command);
                    break;

                case GamePhase.Morning:
                    break;
                    
                case GamePhase.Voting:
                    GameEventHandler.NewVote(splitCommand[0]);
                    break;
                case GamePhase.Lynch:
                    break;
                case GamePhase.Night:
                    AbilityData abilityData = new AbilityData(splitCommand[0], (Abilities)Convert.ToInt32(splitCommand[1]), splitCommand[2]);
                    GameHelper.Commands.Add(abilityData);
                    break;
                default:
                    break;
            }

            playerClient.Socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), playerClient);
        }
    }
}
