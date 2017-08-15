using Pomelo.Data.MySql;
using StockMarketDesktopClient.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StockMarketDesktopClient.Pages.User {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WatchList : Page {
        public WatchList() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            LoadWatchList();
        }

        public void LoadWatchList() {
            MySqlDataReader reader2 = DataBaseHandler.GetData("SELECT DISTINCT StockName FROM WatchList WHERE User = " + DataBaseHandler.UserID);
            List<string> stocks = new List<string>();
            while (reader2.Read()) {
                stocks.Add((string)reader2["StockName"]);
            }
            foreach(string s in stocks) { 
                MySqlDataReader reader = DataBaseHandler.GetData("SELECT StockName, FullName, CurrentPrice, OpeningPriceToday FROM Stock WHERE StockName = '" + s + "'");
                while (reader.Read()) {
                    string Symbol = s;
                    string FullName = (string)reader["FullName"];
                    double Price = (double)reader["CurrentPrice"];
                    double OpeningPrice = (double)reader["OpeningPriceToday"];
                    double RealChangeInPrice = Price - OpeningPrice;
                    double PercentageChange = RealChangeInPrice / OpeningPrice;
                    StackPanel panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;
                    panel.Children.Add(Helper.CreateTextBlock(Symbol, TextAlignment.Left, 100, 20));
                    panel.Children.Add(Helper.CreateTextBlock(FullName, TextAlignment.Left, 250, 20));
                    panel.Children.Add(Helper.CreateTextBlock(Price.ToString().Split('.')[0] + Price.ToString().Split('.')[1].Substring(0, 4), TextAlignment.Left, 100, 20));
                    TextBlock RealChangeInPriceBlock = Helper.CreateTextBlock(0.ToString(), TextAlignment.Left, 100, 20);
                    if (RealChangeInPrice != 0) {
                        RealChangeInPriceBlock = Helper.CreateTextBlock(RealChangeInPrice.ToString().Substring(0, 6), TextAlignment.Left, 100, 20);
                    }
                    if (RealChangeInPrice < 0) {
                        RealChangeInPriceBlock.Foreground = new SolidColorBrush(Colors.Red);
                    } else {
                        RealChangeInPriceBlock.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    panel.Children.Add(RealChangeInPriceBlock);
                    TextBlock PercentageChangeBlock = Helper.CreateTextBlock(PercentageChange.ToString().Substring(0, 4) + "%", TextAlignment.Left, 100, 20);
                    if (RealChangeInPrice < 0) {
                        PercentageChangeBlock.Foreground = new SolidColorBrush(Colors.Red);
                    } else {
                        PercentageChangeBlock.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    panel.Children.Add(PercentageChangeBlock);
                    WatchListList.Items.Add(panel);
                }
            }
        }

        private void ItemClickedListView(object sender, TappedRoutedEventArgs e) {
            string StockName = (((sender as ListView).SelectedItem as StackPanel).Children[0] as TextBlock).Text;
            this.Frame.Navigate(typeof(Pages.User.StockPage), StockName);
        }

        #region HB Menu
        private void HamburgerButton_Click(object sender, RoutedEventArgs e) {
            if (HB_Menu.IsPaneOpen) {
                HB_Menu.IsPaneOpen = false;
            } else {
                HB_Menu.IsPaneOpen = true;
            }
        }

        private void PortfolioMenuClicked(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(Pages.User.Portfolio));
        }

        private void FeauteredStockMenuClicked(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(Pages.FeaturedStock));
        }

        private void WatchListStockMenuClicked(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(Pages.User.WatchList));
        }

        private void AdvanceSearchMenuClicked(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(Pages.User.AdvanceSearch));
        }

        private void AlgoTardingMenuClicked(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(Pages.User.AlgoTrading));
        }
        
    }
    #endregion
}

