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
using System.Net.Sockets;
using System.Threading;
using System.Text.Json;
using Server.Classes;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket? listenSocket;  //  Слушающий сокет -- постоянно активен пока сервер вкл
        private List<ChatMessage> messages;
        public MainWindow()
        {
            InitializeComponent();
            messages = new();
        }
        IPEndPoint? endpoint;
        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(serverIp.Text);
                int port = Convert.ToInt32(serverPort.Text);
                endpoint = new(ip, port);
            }
            catch
            {
                MessageBox.Show("Check start network parameters");
                throw;
            }
            listenSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            new Thread(StartServerMethod).Start(endpoint);
            
        }

        private void StartServerMethod(object? param)
        {
            if (listenSocket is null) return;
            IPEndPoint? endpoint = param as IPEndPoint;
            if (endpoint is null) return;
            Dispatcher.Invoke(() => 
            {
                StartServer.IsEnabled = false;
                StopServer.IsEnabled = true;
                serverLogs.Text = ""; 
            });
            try
            {
                listenSocket.Bind(endpoint);
                listenSocket.Listen(100);
                Dispatcher.Invoke(() => 
                { 
                    serverLogs.Text += serverLogs.Text += "Server started\n";
                    serverStatus.Content = "ON";
                    serverStatus.Foreground = Brushes.Green;
                });

                byte[] buffer = new byte[1024];
                while (true)
                {
                    Socket socket = listenSocket.Accept();
                    StringBuilder sb = new StringBuilder();
                    do
                    {
                        int n = socket.Receive(buffer);
                        sb.Append(System.Text.Encoding.UTF8.GetString(buffer, 0, n));
                    } while (socket.Available > 0);
                    String str = sb.ToString();
                    //Dispatcher.Invoke(() =>
                    //serverLogs.Text += str + "\n");

                    var request = JsonSerializer.Deserialize<ClientRequest>(str);
                    ServerResponse response = new();
                    switch (request?.Action)
                    {
                        case "Message":
                            ChatMessage message = new()
                            {
                                Author = request.Author,
                                Text = request.Text,
                                Moment = request.Moment
                            };
                            messages.Add(message);
                            response.Status = "OK";
                            response.Messages = new() { message };
                            Dispatcher.Invoke(() => serverLogs.Text += $"{request.Moment.ToShortTimeString()} {request.Author}: {request.Text}\n");
                            break;

                        default:
                            response.Status = "Error";
                            break;
                    }

                    str = JsonSerializer.Serialize(response, new JsonSerializerOptions() { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

                    socket.Send(Encoding.UTF8.GetBytes(str));
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Dispose();
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                serverLogs.Text += "Server stopped " + ex.Message + $" {DateTime.Now}" +"\n";
                    serverStatus.Content = "OFF";
                    serverStatus.Foreground = Brushes.Red;
                });
            }
        }
        private void StopServer_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                StartServer.IsEnabled = true;
                StopServer.IsEnabled = false;
            });
            listenSocket?.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (listenSocket is null) return;
            if (listenSocket is not null)
            {
                try
                {
                    listenSocket?.Close();
                    this.Close();
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => serverLogs.Text += "Server stopped " + ex.Message + $" in {DateTime.Now}" +"\n");
                }
            }
        }
    }
}
