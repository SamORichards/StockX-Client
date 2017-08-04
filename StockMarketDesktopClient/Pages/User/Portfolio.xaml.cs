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
using Pomelo.Data.MySql;
using StockMarketDesktopClient.Scripts;
using Windows.UI;

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
            LoadBalance();
            LoadTrades();
            LoadBidsAndOffer();
        }

        private void LoadTrades() {
            TradeList.Items.Clear();
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT Time, StockName, Price, Quantity FROM Trades WHERE BuyerID = " + DataBaseHandler.UserID + " OR SellerID = " + DataBaseHandler.UserID);
            while (reader.Read()) {
                DateTime Time = (DateTime)reader["Time"];
                string StockName = (string)reader["StockName"];
                double Price = (double)reader["Price"];
                int Quantity = (int)reader["Quantity"];
                StackPanel Panel = new StackPanel();
                Panel.Orientation = Orientation.Horizontal;
                Panel.Children.Add(Helper.CreateTextBlock(Time.Date.ToString().Split(' ')[0], TextAlignment.Left, 110, 20));
                Panel.Children.Add(Helper.CreateTextBlock(StockName, TextAlignment.Left, 80, 20));
                Panel.Children.Add(Helper.CreateTextBlock("$" + Price.ToString().Split('.')[0] + "." + Price.ToString().Split('.')[1].Substring(0, 2), TextAlignment.Left, 90, 20));
                Panel.Children.Add(Helper.CreateTextBlock(Quantity.ToString(), TextAlignment.Left, 148, 20));
                TradeList.Items.Add(Panel);
            }
        }

        private void LoadBidsAndOffer() {
            BidsAndOffersList.Items.Clear();
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT Type, StockName, Price, Quantity FROM Pool WHERE User = " + DataBaseHandler.UserID);
            while (reader.Read()) {
                bool Type = (bool)reader["Type"];
                string StockName = (string)reader["StockName"];
                double Price = (double)reader["Price"];
                int Quantity = (int)reader["Quantity"];
                StackPanel Panel = new StackPanel();
                Panel.Orientation = Orientation.Horizontal;
                if (Type) {
                    Panel.Children.Add(Helper.CreateTextBlock("Offer", TextAlignment.Left, 110, 20));
                } else {
                    Panel.Children.Add(Helper.CreateTextBlock("Bid", TextAlignment.Left, 110, 20));
                }
                Panel.Children.Add(Helper.CreateTextBlock(StockName, TextAlignment.Left, 80, 20));
                Panel.Children.Add(Helper.CreateTextBlock("$" + Price.ToString().Split('.')[0] + "." + Price.ToString().Split('.')[1].Substring(0, 2), TextAlignment.Left, 90, 20));
                Panel.Children.Add(Helper.CreateTextBlock(Quantity.ToString(), TextAlignment.Left, 148, 20));
                BidsAndOffersList.Items.Add(Panel);
            }
        }

        private void LoadBalance() {
            double Balance = DataBaseHandler.GetCountDouble("SELECT SUM(Balance) FROM Users WHERE ID = " + DataBaseHandler.UserID);
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT Price, Quantity FROM Pool WHERE Type = 0 AND User = " + DataBaseHandler.UserID);
            double MoneyInPool = 0;
            while (reader.Read()) {
                MoneyInPool += (double)reader["Price"] * (int)reader["Quantity"];
            }
            BalanceText.Text = "Balance: $" + Balance.ToString().Split('.')[0] + "." + Balance.ToString().Split('.')[1].Substring(0, 2);
            if (Balance < 0) {
                BalanceText.Foreground = new SolidColorBrush(Colors.Red);
            } else {
                BalanceText.Foreground = new SolidColorBrush(Colors.Green);
            }
            double FundsAvailable = Balance - MoneyInPool;
            FundsAvailableText.Text = "  Funds Available: $" + FundsAvailable.ToString().Split('.')[0] + "." + FundsAvailable.ToString().Split('.')[1].Substring(0, 2);
            if (FundsAvailable < 0) {
                FundsAvailableText.Text = "  Funds Available: $0";
                FundsAvailableText.Foreground = new SolidColorBrush(Colors.Red);
            }
            MySqlDataReader reader2 = DataBaseHandler.GetData("SELECT DISTINCT StockName FROM StocksInCirculation WHERE OwnerID = " + DataBaseHandler.UserID);
            List<string> StockNames = new List<string>();
            while (reader2.Read()) {
                StockNames.Add((string)reader2["StockName"]);
            }
            double InventoryValue = 0.00d;
            for (int i = 0; i < StockNames.Count; i++) {
                double CurrentPrice = DataBaseHandler.GetCountDouble("SELECT SUM(CurrentPrice) FROM Stock WHERE StockName = '" + StockNames[i] + "'");
                int QuantityOwner = DataBaseHandler.GetCount("SELECT Count(StockID) From StocksInCirculation WHERE OwnerID = " + DataBaseHandler.UserID + " AND " + " StockName = '" + StockNames[i] + "'");
                InventoryValue += CurrentPrice * (double)QuantityOwner;
            }
            string InventoryValueString = InventoryValue.ToString();
            InventoryValueText.Text = "  Invetory Value: $" + InventoryValueString.ToString().Split('.')[0] + "." + InventoryValueString.ToString().Split('.')[1].Substring(0, 2);
        }

        private void LoadInventory() {
            InventoryList.Items.Clear();
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT DISTINCT StockName FROM StocksInCirculation WHERE OwnerID = " + DataBaseHandler.UserID);
            List<string> StockNames = new List<string>();
            while (reader.Read()) {
                StockNames.Add((string)reader["StockName"]);
            }
            for (int i = 0; i < StockNames.Count; i++) {
                Console.WriteLine(i);
                string s = StockNames[i];
                string FullName = "";
                int QuanityOwned = DataBaseHandler.GetCount("SELECT COUNT(StockID) From StocksInCirculation WHERE StockName = '" + s + "' AND OwnerID = " + DataBaseHandler.UserID);
                double AverageCost = DataBaseHandler.GetCountDouble("SELECT AVG(LastTradedPrice) From StocksInCirculation WHERE StockName = '" + s + "' AND OwnerID = " + DataBaseHandler.UserID);
                double CurrentPrice = 0, OpeningPrice = 0, High, Low;
                MySqlDataReader StockReader = DataBaseHandler.GetData("SELECT * FROM Stock WHERE StockName = '" + s + "'");
                while (StockReader.Read()) {
                    FullName = (string)StockReader["FullName"];
                    CurrentPrice = (double)StockReader["CurrentPrice"];
                    OpeningPrice = (double)StockReader["OpeningPriceToday"];
                    High = (double)StockReader["HighToday"];
                    Low = (double)StockReader["LowToday"];
                }
                double Profit = CurrentPrice - AverageCost;
                double RealChangeInPrice = CurrentPrice - OpeningPrice;
                double PercentageChange = RealChangeInPrice / OpeningPrice;
                StackPanel Panel = new StackPanel();
                Panel.Orientation = Orientation.Horizontal;
                Panel.Children.Add(Helper.CreateTextBlock(s, TextAlignment.Left, 100, 20));
                Panel.Children.Add(Helper.CreateTextBlock(FullName, TextAlignment.Left, 250, 20));
                Panel.Children.Add(Helper.CreateTextBlock(CurrentPrice.ToString(), TextAlignment.Left, 200, 20));
                TextBlock RealChangeInPriceBlock = Helper.CreateTextBlock(RealChangeInPrice.ToString().Substring(0, 6), TextAlignment.Left, 100, 20);
                if (RealChangeInPrice < 0) {
                    RealChangeInPriceBlock.Foreground = new SolidColorBrush(Colors.Red);
                } else {
                    RealChangeInPriceBlock.Foreground = new SolidColorBrush(Colors.Green);
                }
                Panel.Children.Add(RealChangeInPriceBlock);

                TextBlock PercentageChangeBlock = Helper.CreateTextBlock(PercentageChange.ToString().Substring(0, 4) + "%", TextAlignment.Left, 100, 20);
                if (RealChangeInPrice < 0) {
                    PercentageChangeBlock.Foreground = new SolidColorBrush(Colors.Red);
                } else {
                    PercentageChangeBlock.Foreground = new SolidColorBrush(Colors.Green);
                }
                Panel.Children.Add(PercentageChangeBlock);
                Panel.Children.Add(Helper.CreateTextBlock(Profit.ToString(), TextAlignment.Left, 200, 20));
                InventoryList.Items.Add(Panel);
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

