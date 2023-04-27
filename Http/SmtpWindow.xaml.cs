using Http.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
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
    /// Interaction logic for SmtpWindow.xaml
    /// </summary>
    public partial class SmtpWindow : Window
    {
        Random _random = new Random();
        private dynamic? emailConfig;
        private readonly DataContext _dataContext;
        private static NpUser _user;

        public SmtpWindow()
        {
            InitializeComponent();
            _dataContext = App.DataContext;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string configFilename = "emailconfig.json";
            if(App.AuthUser is null)
            {
                MessageBox.Show("Unauthorized access!");
                this.Close();
            }
            UserNameTextbox.Text = App.AuthUser.Name;
            UserEmailTextbox.Text = App.AuthUser.Email;
            try
            {
                emailConfig = JsonSerializer.Deserialize<dynamic>(
                System.IO.File.ReadAllText(configFilename)
                );
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show($"{configFilename} не найден.");
                this.Close();
            }
            catch (JsonException ex)
            {
                MessageBox.Show($"{ex.Message}");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
                this.Close();
            }
            if (_user != null)
            {
                if (_user.ConfirmCode == null)
                {
                    MessageBox.Show("Confirmed!");
                }
                else
                {
                    MessageBox.Show("Not Confirmed!");
                }
            }
        }

        private SmtpClient GetSmtpClient()
        {
            if (emailConfig is null) return null!;

            JsonElement gmail = emailConfig.GetProperty("smtp").GetProperty("gmail");
            string host = gmail.GetProperty("host").GetString()!;
            int port = gmail.GetProperty("port").GetInt32();
            string mailbox = gmail.GetProperty("email").GetString()!;
            string password = gmail.GetProperty("password").GetString()!;
            bool ssl = gmail.GetProperty("ssl").GetBoolean();

            return new(host)
            {
                Port = port,
                EnableSsl = ssl,
                Credentials = new NetworkCredential(mailbox, password)
            };
        }

        private void SendTestButton_Click(object sender, RoutedEventArgs e)
        {
            using SmtpClient smtpClient = GetSmtpClient();
            JsonElement gmail = emailConfig.GetProperty("smtp").GetProperty("gmail");
            string mailbox = gmail.GetProperty("email").GetString()!;

            try
            {
                smtpClient.Send(
                    from: mailbox,
                    recipients: "Nikita.bodiu@gmail.com",
                    subject: "Test pv-111",
                    body: "PV-111"
                    );

                MessageBox.Show("Sended");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SendHtmlButton_Click(object sender, RoutedEventArgs e)
        {
            using SmtpClient smtpClient = GetSmtpClient();
            JsonElement gmail = emailConfig!.GetProperty("smtp").GetProperty("gmail");
            string mailbox = gmail.GetProperty("email").GetString()!;

            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(mailbox),
                Body = "<u>Test</u> <i>message</i> from <b style='color:green'>SmtpWindow</b>",
                IsBodyHtml = true,
                Subject = "Test Message"

            };
            mailMessage.To.Add(new MailAddress("Nikita.bodiu@gmail.com"));

            smtpClient.Send(mailMessage);
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (MailadressTextBox.Text == "")
            {
                MessageBox.Show("Email can't be empty!", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageTextBox.Text == "")
            {
                MessageBox.Show("Message can't be empty!", "Warning",
                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;

            }
            if (!MailadressTextBox.Text.Contains("@gmail.com"))
            {
                MessageBox.Show("Invalid input: Email isn't contain! '@gmail'", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using SmtpClient smtpClient = GetSmtpClient();
            if (emailConfig is null) return;
            JsonElement gmail = emailConfig.GetProperty("smtp").GetProperty("gmail");
            string mailbox = gmail.GetProperty("email").GetString()!;


            MailMessage mailMessage = new MailMessage()
            {
                Body = MessageTextBox.Text,
                From = new MailAddress(mailbox),
                Subject = "Test Message"

            };
            mailMessage.To.Add(new MailAddress(MailadressTextBox.Text));
            smtpClient.Send(mailMessage);

            MessageTextBox.Text = MailadressTextBox.Text = "";
        }

        private void SendPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SizeTextBox.Text is null) return;
                

                using SmtpClient smtpClient = GetSmtpClient();
                JsonElement gmail = emailConfig!.GetProperty("smtp").GetProperty("gmail");
                string mailbox = gmail.GetProperty("email").GetString()!;

                MailMessage mailMessage = new MailMessage()
                {
                    From = new MailAddress(mailbox),
                    Body = GetPassword(Convert.ToInt32(SizeTextBox.Text)),
                    IsBodyHtml = true,
                    Subject = "Test Message"

                };
                mailMessage.To.Add(new MailAddress("Nikita.bodiu@gmail.com"));

                smtpClient.Send(mailMessage);
                SizeTextBox.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private String GetPassword(int N)
        {
            if(N == 0) return String.Empty;
            try
            {
                String pass = "";
                for (int i = 0; i <= N; i++)
                {
                    if (_random.Next(1, 10) % 2 == 0)
                    {
                        String tmp = Convert.ToChar(_random.Next(Convert.ToInt32('a'), Convert.ToInt32('z'))).ToString();
                        if (tmp != "o" && tmp != "l" && tmp != "z" && tmp != "s")
                        {
                            pass += tmp;
                        }
                        else
                        {
                            pass += _random.Next(0, 9);
                        }
                    }
                    else
                    {
                        pass += _random.Next(0, 9);
                    }
                }
                return pass;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return String.Empty;
            }
        }


        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // заполняем шаблон письма данными
            String emailPattern = System.IO.File.ReadAllText("email.html");
            String confirmCode = GetPassword(6);
            String emailBody = emailPattern
                .Replace("*name*", UserNameTextbox.Text)
                .Replace("*code*", confirmCode);

            // отправляем письмо с телом emailBody
            using SmtpClient smtpClient = GetSmtpClient();
            MailMessage mailMessage = new()
            {
                From = new MailAddress(emailConfig.GetProperty("smtp").GetProperty("gmail").GetProperty("email").GetString()),
                Body = emailBody,
                IsBodyHtml = true,
                Subject = "Code For Confirm"
            };
            mailMessage.To.Add(new MailAddress(UserEmailTextbox.Text));
            smtpClient.Send(mailMessage);

            // вносим запись в БД
            _dataContext.NpUsers.Add(new()
            {
                Id = Guid.NewGuid(),
                Name = UserNameTextbox.Text,
                Email = UserEmailTextbox.Text,
                ConfirmCode = confirmCode
            });
            _dataContext.SaveChanges();

            // отображаем поле для ввода кода
            ConfirmDockPanel.Visibility = Visibility.Visible;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var NpUser in _dataContext.NpUsers)
            {
                if (NpUser.Name == UserNameTextbox.Text && NpUser.Email == UserEmailTextbox.Text)
                {
                    if (NpUser.ConfirmCode == ConfirmTextbox.Text)
                    {
                        NpUser.ConfirmCode = null;
                        _user = NpUser;
                    }
                }
            }

        }

        private void SendImageButton_Click(object sender, RoutedEventArgs e)
        {
            using SmtpClient smtpClient = GetSmtpClient();
            JsonElement gmail = emailConfig.GetProperty("smtp").GetProperty("gmail");
            String mailbox = gmail.GetProperty("email").GetString()!;
            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(mailbox),
                Body = "<u>Test</u> <i>message</i> from <b style='color:green'>SmtpWindow</b>",
                IsBodyHtml = true,
                Subject = "Test Message"
            };
            mailMessage.To.Add(new MailAddress($"{UserEmailTextbox.Text}"));



            /* Attachment - приложение к письму
                По традициям сетевого программирования, если посылка (пакет)
                содержит в себе приложения (добавочный контент), то
                в нем обязательно должно быть указано тип этого приложения
                (ContentType)
                Для стандартизации введен перечень типов MIME-Types
                Обычно эти типы определяются по расширению файла-приложения
            */
            String filename = "NP.png";
            if (!System.IO.File.Exists(filename))
            {
                MessageBox.Show($"File '{filename}' not found");
                return;
            }
            String mimeType = System.IO.Path.GetExtension(filename)
            switch
            {
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };

            mailMessage.Attachments.Add(
            new Attachment(filename, mimeType));

            try
            {
                smtpClient.Send(mailMessage);
                MessageBox.Show("Sent OK");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sent error '{ex.Message}'");
            }
        }
    }
}