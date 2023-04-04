using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            IPEndPoint endpoint;  // копия - как у сервера                                              // На окне IP - это текст ("127.0.0.1")
            try                                                                                         // Для его перевода в число используется
            {                                                                                           // IPAddress.Parse
                IPAddress ip = IPAddress.Parse(serverIp.Text);                                          // Аналогично - порт
                int port = Convert.ToInt32( serverPort.Text);                                           // парсим число из текста
                endpoint = new(ip, port);                                                               // 
            }                                                                                           // endpoint - комбинация IP и порта
            catch                                                                                       // 
            {
                MessageBox.Show("Check server network parameters");
                return;
            }                                                                                             
                                                                                                        // создаем сокет подключения
            Socket clientSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // адресация IPv4         
            byte[] buffer = new byte[1024];
            //StringBuilder sb = new();
            MemoryStream ms = new();

            try                                                                                         // Двусторонний сокет (и читать, и писать)
            {                                                                                           // Протокол сокета - ТСР
                clientSocket.Connect(endpoint);
                clientSocket.Send( Encoding.UTF8.GetBytes( messageTextBox.Text));

                do
                {
                    //int n = clientSocket.Receive(buffer);
                    //sb.Append(System.Text.Encoding.UTF8.GetString(buffer, 0, n));
                    int n = clientSocket.Receive(buffer);
                    ms.Write(buffer, 0, n);
                } while (clientSocket.Available > 0);
                //String str = sb.ToString();
                //Dispatcher.Invoke(() =>
                //chatLogs.Text += str + "\n");
                String str = Encoding.UTF8.GetString(ms.ToArray());

                chatLogs.Text += str + "\n";

                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Dispose();
            }
            catch (Exception ex)
            {
                chatLogs.Text += ex.Message + "\n";
            }

        }
    }
}