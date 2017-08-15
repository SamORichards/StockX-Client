using System;
using System.Collections.ObjectModel;
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
using Windows.UI;

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
            LoadPrices();
            LoadWatchList();
        }

        private void LoadWatchList() {
            if (DataBaseHandler.GetCount("SELECT COUNT(User) FROM WatchList WHERE User = " + DataBaseHandler.UserID + " AND StockName = '" + StockName + "'") != 0) {
                WatchListButton.Content = "Remove From Watch List";
            }
        }

        private void LoadPrices() {
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT FullName, CurrentPrice, OpeningPriceToday, VolumeTraded FROM Stock WHERE StockName = '" + StockName + "'");
            while (reader.Read()) {
                string FullName = (string)reader["FullName"];
                double Price = (double)reader["CurrentPrice"];
                double OpeningPrice = (double)reader["OpeningPriceToday"];
                double RealChangeInPrice = Price - OpeningPrice;
                double PercentageChange = RealChangeInPrice / OpeningPrice;
                int VolumeTraded = (int)reader["VolumeTraded"];
                StockNameTitle.Text = FullName;
                if (RealChangeInPrice != 0) {
                    ChangeInPricePercentage.Text = "Change In Price: " + PercentageChange.ToString().Split('.')[0] + "." + PercentageChange.ToString().Split('.')[1].Substring(0, 4) + "%";
                    ChangeInPrice.Text = "Change In Price: " + RealChangeInPrice.ToString().Split('.')[0] + "." + RealChangeInPrice.ToString().Split('.')[1].Substring(0, 4) + "%";
                } else {
                    ChangeInPrice.Text = "Change In Price: 0.00%";
                    ChangeInPricePercentage.Text = "Change In Price: 0.00%";
                }
                if (RealChangeInPrice < 0) {
                    ChangeInPrice.Foreground = new SolidColorBrush(Colors.Red);
                    ChangeInPricePercentage.Foreground = new SolidColorBrush(Colors.Red);
                } else {
                    ChangeInPrice.Foreground = new SolidColorBrush(Colors.Green);
                    ChangeInPricePercentage.Foreground = new SolidColorBrush(Colors.Green);
                }
                CurrentPriceBlock.Text = "Current Price: " + Price.ToString().Split('.')[0] + "." + Price.ToString().Split('.')[1].Substring(0, 4);
                VolumeTradedToday.Text = "Volume Traded: " + VolumeTraded.ToString();
                SellButtonObject.Width = 250;
                BuyButtonObject.Width = SellButtonObject.Width;
                WatchListButton.Width = SellButtonObject.Width;
            }
        }
        public class test {
            public ObservableCollection<PriceHis> PriceHistory {get; set;}
            public test() {
                PriceHistory = new ObservableCollection<PriceHis>();
            }
        }

        private void LoadChart() {
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT Time, Price FROM PricingHistory WHERE StockName = '" + StockName + "' LIMIT 20");
            test Test = new test();
            ViewModel = new ObservableCollection<test>();
            while (reader.Read()) {
                Test.PriceHistory.Add(new PriceHis() { Time = ((DateTime)reader["Time"]).ToString().Split(' ')[1], Price = (double)reader["Price"] });
            }
            ViewModel.Add(Test);
            Line.Series[0].ItemsSource = ViewModel[0].PriceHistory;
        }
        public ObservableCollection<test> ViewModel { get; set; }

        private void SellButton(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(Pages.User.BuySellPage), StockName + "/" + (int)BidOffer.offer);
        }

        private void BuyButton(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(Pages.User.BuySellPage), StockName + "/" + (int)BidOffer.bid);
        }

        private void WatchListClick(object sender, RoutedEventArgs e) {
            if (DataBaseHandler.GetCount("SELECT COUNT(User) FROM WatchList WHERE User = " + DataBaseHandler.UserID + " AND StockName = '" + StockName + "'") == 0) {
                DataBaseHandler.SetData(string.Format("INSERT INTO WatchList (User, StockName) VALUES ({0}, '{1}')", DataBaseHandler.UserID, StockName));
                WatchListButton.Content = "Remove From Watch List";
            } else {
                DataBaseHandler.SetData(string.Format("DELETE FROM WatchList WHERE User = {0} AND StockName = '{1}'", DataBaseHandler.UserID, StockName));
                WatchListButton.Content = "Add To Watch List";
            }
        }
    }
}
