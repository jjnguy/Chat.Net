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

        private List<Message> _history;

        public MainWindow()
        {
            _history = new List<Message>();
            _client = new DaClient(new ConsoleMessageLogger());
            _client.Connect("WPF", "anna", message =>
            {
                _history.Add(message);
                Dispatcher.BeginInvoke(new RefreshUiDel(RefreshUi));
            });

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var message = new Message
            {
                Data = this.MessageText.Text,
                Type = MessageType.Message,
            };
            _client.Send(message);
            _history.Add(message);
            RefreshUi();
        }

        private void RefreshUi()
        {
            this.MessagesPanel.Children.Clear();
            foreach (var message in _history)
            {
                var messagePanel = new ItemsControl
                {
                };
                messagePanel.Items.Add(new Label
                {
                    Content = "From: " + (string.IsNullOrWhiteSpace(message.ClientName) ? "me" : message.ClientName),
                });
                messagePanel.Items.Add(new Label
                {
                    Content = message.Data,
                });
                this.MessagesPanel.Children.Add(messagePanel);
            }
        }

        delegate void RefreshUiDel();
    }
}
