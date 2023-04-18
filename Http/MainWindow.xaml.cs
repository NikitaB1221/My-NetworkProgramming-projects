using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace Http
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HttpClient httpClient;

        public MainWindow()
        {
            InitializeComponent();
            httpClient = new HttpClient();
        }

        private async void get1Button_Click(object sender, RoutedEventArgs e)
        {
            String result = await httpClient.GetStringAsync(url1TextBox.Text);
            resultTextBlock.Text = result;
        }

        private async void get2Button_Click(object sender, RoutedEventArgs e)
        {
            HttpRequestMessage request = new(   // Более полная форма запроса
                HttpMethod.Get,                 // Метод
                url2TextBox.Text);              // URL

            HttpResponseMessage response =            // Более полная форма
                await httpClient.SendAsync(request);  // ответа

            printResponse(response);
        }

        private async void head3Button_Click(object sender, RoutedEventArgs e)
        {
            var response = await httpClient.SendAsync(
                new HttpRequestMessage(
                    HttpMethod.Head,
                    url3TextBox.Text));

            printResponse(response);
        }

        private async void printResponse(HttpResponseMessage response)
        {
            resultTextBlock.Text = $"HTTP/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}\n";
            foreach (var header in response.Headers)
            {   // var - KeyValuePair<string,IEnumerable<string>>
                String headerString = header.Key + ": ";
                foreach (string value in header.Value)
                {
                    headerString += value + " ";
                }
                resultTextBlock.Text += $"{headerString}\n";
            }
            resultTextBlock.Text += "------------------------------------\n";
            resultTextBlock.Text += await response.Content.ReadAsStringAsync();
            resultTextBlock.Text += "\n------------------------------------\n";
        }

        private async void searchResponse(HttpResponseMessage response)
        {
            // Дз за 11.04
        }

        private async void options4Button_Click(object sender, RoutedEventArgs e)
        {
            var response = await httpClient.SendAsync(
                new HttpRequestMessage(
                    HttpMethod.Options,
                    url4TextBox.Text));

            printResponse(response);
        }

        private async void get5Button_Click(object sender, RoutedEventArgs e)
        {
            var response = await httpClient.SendAsync(
                new HttpRequestMessage(
                    HttpMethod.Get,
                    url5TextBox.Text));

            printResponse(response);
        }

        private async void get6Button_Click(object sender, RoutedEventArgs e)
        {
            var response = await httpClient.SendAsync(
                new HttpRequestMessage(
                    HttpMethod.Get,
                    url5TextBox.Text));

            searchResponse(response);
        }
    }
}