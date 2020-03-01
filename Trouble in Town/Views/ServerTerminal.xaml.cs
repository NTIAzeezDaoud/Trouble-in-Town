using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Net;
using Trouble_in_Town.ServerClient;

namespace Trouble_in_Town.Views
{
    /// <summary>
    /// Interaction logic for ServerTerminal.xaml
    /// </summary>
    public partial class ServerTerminal : UserControl
    {
        Server server;
        public ServerTerminal()
        {
            InitializeComponent();

            RunningWindows.MainWindow.Title = "Server Terminal";

            Login loginWindow = (Login)RunningWindows.GetWindow("Login");
            RunningWindows.AddWindow(this);
            string address = loginWindow.portText.Text;
            string port = loginWindow.portText.Text;

            if (string.IsNullOrEmpty(address))
                address = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            if (string.IsNullOrEmpty(port))
                port = "13000";
            
            server = new Server(address, Convert.ToInt32(port));
        }

        public void Terminal(string line){
            Dispatcher.Invoke(() =>
            {
                terminal.Text += line + "\n";
            });
        }

        public void UpdateInformation(string address, int port)
        {
            Dispatcher.Invoke(() =>
            {
                addressLabel.Content += address;
                portLabel.Content += port.ToString();
            });
        }

        private void StartGameButton(object sender, RoutedEventArgs e)
        {
            GameplayServer.BeginGame();
        }
    }
}
