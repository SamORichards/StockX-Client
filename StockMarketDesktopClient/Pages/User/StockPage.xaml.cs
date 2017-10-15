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
        public DateTime Time { get; set; }
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
            int QuantityOwned = DataBaseHandler.GetCount("SELECT SUM(Quantity) From Inventories WHERE StockName = '" + StockName + "' AND UserID = " + DataBaseHandler.UserID);
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT * FROM Stock WHERE StockName = '" + StockName + "'");
            while (reader.Read()) {
                string FullName = (string)reader["FullName"];
                double Price = (double)reader["CurrentPrice"];
                string Description = (string)reader["Description"];
                double OpeningPrice = (double)reader["OpeningPriceToday"];
                double RealChangeInPrice = Price - OpeningPrice;
                double PercentageChange = RealChangeInPrice / OpeningPrice;
                int VolumeTraded = (int)reader["VolumeTraded"];
                StockNameTitle.Text = FullName;
                DescriptionText.Text = Description;
                if (RealChangeInPrice != 0) {
                    ChangeInPricePercentage.Text = "Change In Price: " + PercentageChange.ToString().Split('.')[0] + "." + PercentageChange.ToString().Split('.')[1].Substring(0, 4) + "%";
                    ChangeInPrice.Text = "Change In Price: " + RealChangeInPrice.ToString().Split('.')[0] + "." + RealChangeInPrice.ToString().Split('.')[1].Substring(0, 4);
                } else {
                    ChangeInPrice.Text = "Change In Price: 0.00";
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
                if (QuantityOwned > 0) {
                    VolumeTradedToday.Text += "\nStock Owned: " + QuantityOwned;
                }
                SellButtonObject.Width = 250;
                BuyButtonObject.Width = SellButtonObject.Width;
                WatchListButton.Width = SellButtonObject.Width;
            }
        }
        public class PriceChartList {
            public ObservableCollection<PriceHis> PriceHistory { get; set; }
            public PriceChartList() {
                PriceHistory = new ObservableCollection<PriceHis>();
            }
        }

        private void LoadChart() {
            TenClick(null, null);
        }

        public ObservableCollection<PriceChartList> ViewModel { get; set; }

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

        private void AllTimeClick(object sender, RoutedEventArgs e) {
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT Time, Price FROM PricingHistory WHERE StockName = '" + StockName + "'");
            PriceChartList Test = new PriceChartList();
            ViewModel = new ObservableCollection<PriceChartList>();
            while (reader.Read()) {
                Test.PriceHistory.Add(new PriceHis() { Time = (DateTime)reader["Time"], Price = (double)reader["Price"] });
            }
            ViewModel.Add(Test);
            Line.Series[0].ItemsSource = ViewModel[0].PriceHistory;
        }

        private void MonthClick(object sender, RoutedEventArgs e) {
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT Time, Price FROM PricingHistory WHERE StockName = '" + StockName + "' AND Time > '" + DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd HH:mm:ss") + "'");
            PriceChartList Test = new PriceChartList();
            ViewModel = new ObservableCollection<PriceChartList>();
            while (reader.Read()) {
                Test.PriceHistory.Add(new PriceHis() { Time = (DateTime)reader["Time"], Price = (double)reader["Price"] });
            }
            TimeSpan gap = new TimeSpan(6, 0, 0);
            Test.PriceHistory = PricingThinner(Test.PriceHistory, gap);
            ViewModel.Add(Test);
            Line.Series[0].ItemsSource = ViewModel[0].PriceHistory;
        }

        private void WeekClick(object sender, RoutedEventArgs e) {
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT Time, Price FROM PricingHistory WHERE StockName = '" + StockName + "' AND Time > '" + DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss") + "'");
            PriceChartList Test = new PriceChartList();
            ViewModel = new ObservableCollection<PriceChartList>();
            while (reader.Read()) {
                Test.PriceHistory.Add(new PriceHis() { Time = (DateTime)reader["Time"], Price = (double)reader["Price"] });
            }
            TimeSpan gap = new TimeSpan(1, 0, 0);
            Test.PriceHistory = PricingThinner(Test.PriceHistory, gap);
            ViewModel.Add(Test);
            Line.Series[0].ItemsSource = ViewModel[0].PriceHistory;
        }

        private void DayClick(object sender, RoutedEventArgs e) {
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT Time, Price FROM PricingHistory WHERE StockName = '" + StockName + "' AND Time > '" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss") + "'");
            PriceChartList Test = new PriceChartList();
            ViewModel = new ObservableCollection<PriceChartList>();
            while (reader.Read()) {
                Test.PriceHistory.Add(new PriceHis() { Time = (DateTime)reader["Time"], Price = (double)reader["Price"] });
            }
            TimeSpan gap = new TimeSpan(0, 10, 0);
            Test.PriceHistory = PricingThinner(Test.PriceHistory, gap);
            ViewModel.Add(Test);
            Line.Series[0].ItemsSource = ViewModel[0].PriceHistory;
        }

        private void TwelveClick(object sender, RoutedEventArgs e) {
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT Time, Price FROM PricingHistory WHERE StockName = '" + StockName + "' AND Time > '" + DateTime.Now.AddHours(-12).ToString("yyyy-MM-dd HH:mm:ss") + "'");
            PriceChartList Test = new PriceChartList();
            ViewModel = new ObservableCollection<PriceChartList>();
            while (reader.Read()) {
                Test.PriceHistory.Add(new PriceHis() { Time = (DateTime)reader["Time"], Price = (double)reader["Price"] });
            }
            TimeSpan gap = new TimeSpan(0, 2, 0);
            Test.PriceHistory = PricingThinner(Test.PriceHistory, gap);
            ViewModel.Add(Test);
            Line.Series[0].ItemsSource = ViewModel[0].PriceHistory;
        }

        private void HourClick(object sender, RoutedEventArgs e) {
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT Time, Price FROM PricingHistory WHERE StockName = '" + StockName + "' AND Time > '" + DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss") + "'");
            PriceChartList Test = new PriceChartList();
            ViewModel = new ObservableCollection<PriceChartList>();
            while (reader.Read()) {
                Test.PriceHistory.Add(new PriceHis() { Time = (DateTime)reader["Time"], Price = (double)reader["Price"] });
            }
            TimeSpan gap = new TimeSpan(0, 0, 10);
            Test.PriceHistory = PricingThinner(Test.PriceHistory, gap);
            ViewModel.Add(Test);
            Line.Series[0].ItemsSource = ViewModel[0].PriceHistory;
        }

        private void TenClick(object sender, RoutedEventArgs e) {
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT Time, Price FROM PricingHistory WHERE StockName = '" + StockName + "' AND Time > '" + DateTime.Now.AddMinutes(-10).ToString("yyyy-MM-dd HH:mm:ss") + "'");
            PriceChartList Test = new PriceChartList();
            ViewModel = new ObservableCollection<PriceChartList>();
            while (reader.Read()) {
                Test.PriceHistory.Add(new PriceHis() { Time = (DateTime)reader["Time"], Price = (double)reader["Price"] });
            }
            ViewModel.Add(Test);
            Line.Series[0].ItemsSource = ViewModel[0].PriceHistory;
        }

        static ObservableCollection<PriceHis> PricingThinner(ObservableCollection<PriceHis> Times, TimeSpan Gap) {
            List<PriceHis> ToBeDeleted = new List<PriceHis>();
            DateTime last = DateTime.MinValue;
            for (int i = 0; i < Times.Count; i++) {
                if (i == 0) {
                    last = Times[i].Time;
                    continue;
                }
                if ((Times[i].Time - last) < Gap) {
                    ToBeDeleted.Add(Times[i]);
                } else {
                    last = Times[i].Time;
                }
            }
            foreach (PriceHis time in ToBeDeleted) {
                Times.Remove(time);
            }
            return Times;
        }
    }
}
