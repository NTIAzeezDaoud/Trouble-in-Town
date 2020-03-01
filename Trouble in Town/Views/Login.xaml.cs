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

namespace Trouble_in_Town.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
            RunningWindows.MainWindow.Title = "Login";
        }

        private void join_button(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            RunningWindows.AddWindow(this);
            RunningWindows.MainWindow.DataContext = new GameScreen();
        }

        private void host_button(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            RunningWindows.AddWindow(this);
            RunningWindows.MainWindow.DataContext = new ServerTerminal();
        }
    }
}
