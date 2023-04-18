using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
    /// Логика взаимодействия для CoinCapWindow.xaml
    /// </summary>
    public partial class CoinCapWindow : Window
    {
        public ObservableCollection<Asset> Assets { get; set; }
        private HttpClient _httpClient = new();
        private int indexShown;
        Color color;

        Random rnd = new Random();

        public CoinCapWindow()
        {
            InitializeComponent();
            Assets = new();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAssets();
        }

        private async void LoadAssets()
        {
            var assetsResponse = await _httpClient.GetFromJsonAsync<AssetsResponse>("https://api.coincap.io/v2/assets");
            if (assetsResponse is null)
            {
                MessageBox.Show("JSON error", "JSON error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            indexShown = 4;
            ShowHistory(assetsResponse.data[indexShown]);
            Assets.Clear();
            foreach (Asset asset in assetsResponse.data)
            {
                Assets.Add(asset); 
            }
            AssetsListView.SelectedIndex = indexShown;
            RateHistoryLabel.Content = $"Rate history: {Assets[AssetsListView.SelectedIndex].name}";

        }

        private async void ShowHistory(Asset asset)
        {
            if (asset is null) return;
            String url = $"https://api.coincap.io/v2/assets/{asset.id}/history?interval=d1";
            var historyResponse = await _httpClient.GetFromJsonAsync<HistoryResponse>(url);
            if (historyResponse is null)
            {
                MessageBox.Show("JSON history error", "JSON history error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            long minTime, maxTime;
            double minRate, maxRate;
            minTime = historyResponse.data.Min(r => r.time);
            maxTime = historyResponse.data.Max(r => r.time);
            minRate = historyResponse.data.Min(r => r.priceUsd);
            maxRate = historyResponse.data.Max(r => r.priceUsd);
            double graphHeight = GraphCanvas.ActualHeight - 20;
            double graphWidth = GraphCanvas.ActualWidth;
            const double offset = 0.05;  // отступ от границ графика (сверху и снизу)

            // 2. Проходим массив данных, пересчитываем точку на графике, формируем линии
            double x1, y1, x2, y2;
            double kx = graphWidth / (maxTime - minTime); 
            double h = graphHeight * (1 - offset);
            double ky = graphHeight / (maxRate - minRate) * (1 - 2 * offset);



            x1 = // шкала времени от minTime до maxTime ==> 0 .. graphWidth
                           (historyResponse.data[0].time - minTime) * kx;
            y1 = // шкала Y перевернута и используем отступы offset от "краев"
                           h - (historyResponse.data[0].priceUsd - minRate) * ky;

            MaxPriceUsdLabel.Content = $"{maxRate} $";
            MinPriceUsdLabel.Content = $"{minRate} $";

            color = Color.FromRgb((byte)rnd.Next(10, 115), (byte)rnd.Next(10, 115), (byte)rnd.Next(10, 115));

            foreach (Rate rate in historyResponse.data)
            {
                x2 = (rate.time - minTime) * kx;
                y2 = h - (rate.priceUsd - minRate) * ky;
                DrawLine(x1, y1, x2, y2, color);
                x1 = x2;
                y1 = y2;
            }

            
        }



        private void DrawLine(double fromX, double fromY, double toX, double toY,Color color)
        {
            
            Line line = new()
            {
                X1 = fromX,
                Y1 = fromY,
                X2 = toX,
                Y2 = toY,
        
                Stroke = new SolidColorBrush(color),
                StrokeThickness = 2
            };
            GraphCanvas.Children.Add(line);
        }

        private void AssetsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (indexShown == AssetsListView.SelectedIndex)
            {
                return;
            }
            GraphCanvas.Children.Clear();
            ShowHistory(Assets[AssetsListView.SelectedIndex]);
            indexShown = AssetsListView.SelectedIndex;
            RateHistoryLabel.Content = $"Rate history: {Assets[AssetsListView.SelectedIndex].name}";
        }
    }

    //  ORM
    public class Asset
    {
        public string id { get; set; }
        public string rank { get; set; }
        public string symbol { get; set; }
        public string name { get; set; }
        public string supply { get; set; }
        public string maxSupply { get; set; }
        public string marketCapUsd { get; set; }
        public string volumeUsd24Hr { get; set; }
        public double priceUsd { get; set; }
        public string changePercent24Hr { get; set; }
        public string vwap24Hr { get; set; }
        public string explorer { get; set; }
    }

    public class AssetsResponse
    {
        public List<Asset> data { get; set; }

        public long timestamp { get; set; }
    }

    public class Rate
    {
        public double priceUsd { get; set; }
        public long time { get; set; }
        public DateTime date { get; set; }
    }

    public class HistoryResponse
    {
        public List<Rate> data { get; set; }

        public long timestamp { get; set; }

    }
}
