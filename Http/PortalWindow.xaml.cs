using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Shapes;

namespace Http
{
    /// <summary>
    /// Логика взаимодействия для PortalWindow.xaml
    /// </summary>
    public partial class PortalWindow : Window
    {
        String currentDir;
        int httpPosition;
        String projectDir;
        public PortalWindow()
        {
            InitializeComponent();
            currentDir = Directory.GetCurrentDirectory();
            httpPosition = currentDir.IndexOf("Http");
            projectDir = currentDir[..httpPosition];    
        }

        private void ChatServer_Click(object sender, RoutedEventArgs e)
        {
            String serverPath = @"Server\bin\Debug\net6.0-windows\Server.exe";
            Process serverProcess = Process.Start(projectDir + serverPath);
            //MessageBox.Show(projectDir);
        }

        private void ChatClient_Click(object sender, RoutedEventArgs e)
        {
            String clientPath = @"Client\bin\Debug\net6.0-windows\Client.exe";
            Process clientProcess = Process.Start(projectDir + clientPath);
            //MessageBox.Show(projectDir);
        }

        private void HttpRequest_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
        }

        private void APIRequest_Click(object sender, RoutedEventArgs e)
        {
            new ApiWindow().Show();
        }

        private void APIRequest2_Click(object sender, RoutedEventArgs e)
        {
            new CoinCapWindow().Show();
        }

        private void SmtpButton_Click(object sender, RoutedEventArgs e)
        {
            new SmtpWindow().Show();
        }
    }
}
