using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using StockMarketDesktopClient.Scripts;
using Pomelo.Data.MySql;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StockMarketDesktopClient.Pages.User {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public class PriceHis {
        public string Time { get; set; }
        public double Price { get; set; }
    }
    public sealed partial class StockPage : Page {
        string StockName;
        public StockPage() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            StockName = (string)e.Parameter;
            LoadChart();
        }

        private void LoadChart() {
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT Time, Price FROM PricingHistory WHERE StockName = '" + StockName + "' LIMIT 20");
            List<PriceHis> Prices = new List<PriceHis>();
            while (reader.Read()) {
                Prices.Add(new PriceHis() { Time = ((DateTime)reader["Time"]).ToString(), Price = (double)reader["Price"]});
            }

            (LineChart.Series[0] as LineSeries).ItemsSource = Prices;
        }
    }
}
