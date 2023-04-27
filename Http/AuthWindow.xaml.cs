using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoginTextbox.Focus();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            String passwordHash = Hash(PasswordPasswordbox.Password);
            App.AuthUser = App.DataContext.NpUsers.FirstOrDefault(user => user.Login == LoginTextbox.Text && user.Password == passwordHash);
            if (App.AuthUser is null)
            {
                MessageBox.Show("Вход отклонен");
            }
            else
            {
                this.Hide();
                new PortalWindow().ShowDialog();
                PasswordPasswordbox.Password = "";
                this.Show();
            }
        }

        private string Hash(string str)
        {
            using MD5 md5 = MD5.Create();
            return Convert.ToHexString(
                        md5.ComputeHash(
                            Encoding.UTF8.GetBytes(str)));
        }
    }
}
/* Аутендификация - подтверждение (проверка логина/пароля/...)
 * Авторизация - проверка прав доступа данного пользователя к данному рессурсу
 */