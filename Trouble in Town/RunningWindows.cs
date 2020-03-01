using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Trouble_in_Town.Views;

namespace Trouble_in_Town
{
    public static class RunningWindows
    {
        public static MainWindow MainWindow;
        public static GameScreen GameScreen;

        public static List<Tuple<string, object>> windows = new List<Tuple<string, object>>();

        public static void AddWindow(object window) => windows.Add(new Tuple<string, object>(window.GetType().Name, window));

        public static object GetWindow(string name)
        {
            foreach (var item in windows)
            {
                if (item.Item1 == name)
                {
                    return item.Item2;
                }
            }
            return null;
        }
    }
}
