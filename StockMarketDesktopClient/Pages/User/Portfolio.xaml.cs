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
using Microsoft.Data.Sqlite;
using StockMarketDesktopClient.Scripts;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StockMarketDesktopClient.Pages.User {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Portfolio : Page {
        public Portfolio() {
            this.InitializeComponent();
        }

        #region HB Menu
        private void HamburgerButton_Click(object sender, RoutedEventArgs e) {
            if (HB_Menu.IsPaneOpen) {
                HB_Menu.IsPaneOpen = false;
            } else {
                HB_Menu.IsPaneOpen = true;
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            LoadInventory();
        }

        private void LoadInventory() {
            InventoryList.Items.Clear();
            SqliteDataReader reader = DataBaseHandler.GetData("SELECT DISTINCT StockName FROM StocksInCirculation WHERE OwnerID = " + DataBaseHandler.UserID);
            List<string> StockNames = new List<string>();
            while (reader.Read()) {
                StockNames.Add((string)reader["StockName"]);
            }
            foreach (string s in StockNames) {
                int QuanityOwned = DataBaseHandler.GetCount("SELECT COUNT(StockID) From StocksInCirculation WHERE StockName = '" + s + "' AND OwnerID = " + DataBaseHandler.UserID);
                float AverageCost = DataBaseHandler.GetCount("SELECT AVG(LastTradedPrice) From StocksInCirculation WHERE StockName = '" + s + "' AND OwnerID = " + DataBaseHandler.UserID);
                float CurrentPrice = 0, OpeningPrice, High, Low;
                SqliteDataReader StockReader = DataBaseHandler.GetData("SELECT * FROM Stock WHERE StockName = '" + s +"'");
                while (StockReader.Read()) {
                    CurrentPrice = (float)StockReader["CurrentPrice"];
                    OpeningPrice = (float)StockReader["OpeningPriceToday"];
                    High = (float)StockReader["HighToday"];
                    Low = (float)StockReader["LowToday"];
                }
                float Profit = CurrentPrice - AverageCost;
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

        private void InventoryItemClick(object sender, ItemClickEventArgs e) {
            
        }
    }
    #endregion
}

