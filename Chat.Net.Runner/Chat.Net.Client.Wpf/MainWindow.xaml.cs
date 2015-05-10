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
using Chat.Net.Protocol;

namespace Chat.Net.Client.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DaClient _client;

        public MainWindow()
        {
            _client = new DaClient(new ConsoleMessageLogger());
            _client.Connect("a_room", message =>
            {
                Console.WriteLine("Got a message");
            });

            InitializeComponent();
        }
    }
}
