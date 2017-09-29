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
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StockMarketDesktopClient.Pages.User {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

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
        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            LoadInventory();
            LoadBalance();
            LoadTrades();
            LoadBidsAndOffer();
        }

        private void LoadTrades() {
            TradeList.Items.Clear();
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT Time, StockName, BuyerID, Price, Quantity FROM Trades WHERE BuyerID = " + DataBaseHandler.UserID + " OR SellerID = " + DataBaseHandler.UserID);
            while (reader.Read()) {
                DateTime Time = (DateTime)reader["Time"];
                string StockName = (string)reader["StockName"];
                int BuyerID = (int)reader["BuyerID"];
                double Price = (double)reader["Price"];
                int Quantity = (int)reader["Quantity"];
                StackPanel Panel = new StackPanel();
                Panel.Orientation = Orientation.Horizontal;
                string Date = Time.Date.ToString().Split(' ')[0];
                Date = Date.Split('/')[0] + "/" + Date.Split('/')[1] + "/" + Date.Split('/')[2][2] + Date.Split('/')[2][3];
                Panel.Children.Add(Helper.CreateTextBlock(Date, TextAlignment.Left, 110, 20));
                if (BuyerID == DataBaseHandler.UserID) {
                    Panel.Children.Add(Helper.CreateTextBlock("Bid", TextAlignment.Left, 55, 20));
                } else {
                    Panel.Children.Add(Helper.CreateTextBlock("Offer", TextAlignment.Left, 55, 20));
                }
                Panel.Children.Add(Helper.CreateTextBlock(StockName, TextAlignment.Left, 80, 20));
                Panel.Children.Add(Helper.CreateTextBlock("$" + Price.ToString().Split('.')[0] + "." + Price.ToString().Split('.')[1].Substring(0, 2), TextAlignment.Left, 75, 20));
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
                    Panel.Children.Add(Helper.CreateTextBlock("Offer", TextAlignment.Left, 55, 20));
                } else {
                    Panel.Children.Add(Helper.CreateTextBlock("Bid", TextAlignment.Left, 55, 20));
                }
                Panel.Children.Add(Helper.CreateTextBlock(StockName, TextAlignment.Left, 80, 20));
                TextBlock PriceBox = Helper.CreateTextBlock("$" + Price.ToString(), TextAlignment.Left, 75, 20);
                if (Price.ToString().Contains('.')) {
                    Helper.CreateTextBlock("$" + Price.ToString().Split('.')[0] + "." + Price.ToString().Split('.')[1].Substring(0, 2), TextAlignment.Left, 75, 20);
                }
                Panel.Children.Add(PriceBox);
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
            if (Balance.ToString().Contains('.')) {
                BalanceText.Text = "Balance: $" + Balance.ToString().Split('.')[0] + "." + Balance.ToString().Split('.')[1].Substring(0, 2);
            } else {
                BalanceText.Text = "Balance: $" + Balance.ToString();
            }
            if (Balance < 0) {
                BalanceText.Foreground = new SolidColorBrush(Colors.Red);
            } else {
                BalanceText.Foreground = new SolidColorBrush(Colors.Green);
            }
            double FundsAvailable = Balance - MoneyInPool;
            FundsAvailableText.Text = "  Funds Available: $" + Math.Round(FundsAvailable, 2);
            if (FundsAvailable < 0) {
                FundsAvailableText.Text = "  Funds Available: $0";
                FundsAvailableText.Foreground = new SolidColorBrush(Colors.Red);
            }
            MySqlDataReader reader2 = DataBaseHandler.GetData("SELECT DISTINCT StockName FROM Inventories WHERE UserID = " + DataBaseHandler.UserID);
            List<string> StockNames = new List<string>();
            while (reader2.Read()) {
                StockNames.Add((string)reader2["StockName"]);
            }
            double InventoryValue = 0.00d;
            for (int i = 0; i < StockNames.Count; i++) {
                double CurrentPrice = DataBaseHandler.GetCountDouble("SELECT SUM(CurrentPrice) FROM Stock WHERE StockName = '" + StockNames[i] + "'");
                int QuantityOwner = DataBaseHandler.GetCount("SELECT SUM(Quantity) From Inventories WHERE UserID = " + DataBaseHandler.UserID + " AND " + " StockName = '" + StockNames[i] + "'");
                InventoryValue += CurrentPrice * (double)QuantityOwner;
            }
            InventoryValueText.Text = "  Invetory Value: $" + Math.Round(InventoryValue, 2);
        }

        private void LoadInventory() {
            InventoryList.Items.Clear();
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT DISTINCT StockName FROM Inventories WHERE Quantity > 0 AND UserID = " + DataBaseHandler.UserID);
            List<string> StockNames = new List<string>();
            while (reader.Read()) {
                StockNames.Add((string)reader["StockName"]);
            }
            for (int i = 0; i < StockNames.Count; i++) {
                Console.WriteLine(i);
                string s = StockNames[i];
                string FullName = "";
                int QuantityOwned = DataBaseHandler.GetCount("SELECT SUM(Quantity) From Inventories WHERE StockName = '" + s + "' AND UserID = " + DataBaseHandler.UserID);
                double AverageCost = DataBaseHandler.GetCountDouble("SELECT AVG(LastTradedPrice) From Inventories WHERE StockName = '" + s + "' AND UserID = " + DataBaseHandler.UserID);
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
                Panel.Children.Add(Helper.CreateTextBlock(QuantityOwned.ToString(), TextAlignment.Left, 100, 20));
                Panel.Children.Add(Helper.CreateTextBlock(Math.Round(CurrentPrice, 2).ToString(), TextAlignment.Left, 200, 20));
                TextBlock RealChangeInPriceBlock = Helper.CreateTextBlock(Math.Round(RealChangeInPrice, 2).ToString(), TextAlignment.Left, 100, 20);
                if (RealChangeInPrice < 0) {
                    RealChangeInPriceBlock.Foreground = new SolidColorBrush(Colors.Red);
                } else {
                    RealChangeInPriceBlock.Foreground = new SolidColorBrush(Colors.Green);
                }
                Panel.Children.Add(RealChangeInPriceBlock);

                TextBlock PercentageChangeBlock = Helper.CreateTextBlock(Math.Round(PercentageChange, 2) + "%", TextAlignment.Left, 100, 20);
                if (RealChangeInPrice < 0) {
                    PercentageChangeBlock.Foreground = new SolidColorBrush(Colors.Red);
                } else {
                    PercentageChangeBlock.Foreground = new SolidColorBrush(Colors.Green);
                }
                Panel.Children.Add(PercentageChangeBlock);
                Panel.Children.Add(Helper.CreateTextBlock(Math.Round(Profit, 2).ToString(), TextAlignment.Left, 200, 20));
                InventoryList.Items.Add(Panel);
            }
        }


        private void ScreenChanged(object sender, SizeChangedEventArgs e) {
            if (PagePane.ActualWidth < 1400 && screenState != ScreenState.SmallDesktop) {
                screenState = ScreenState.SmallDesktop;
                foreach (TextBlock block in (InventoryList.Header as StackPanel).Children) {
                    block.FontSize = block.FontSize / 1.5;
                    block.Width = block.Width / 1.5;
                }
                foreach (StackPanel panel in InventoryList.Items) {
                    foreach (TextBlock block in panel.Children) {
                        block.FontSize = block.FontSize / 1.75;
                        block.Width = block.Width / 1.75;
                    }
                }
                foreach (TextBlock block in (TradeList.Header as StackPanel).Children) {
                    block.FontSize = block.FontSize / 1.5;
                    block.Width = block.Width / 1.5;
                }
                foreach (StackPanel panel in TradeList.Items) {
                    foreach (TextBlock block in panel.Children) {
                        block.FontSize = block.FontSize / 1.5;
                        block.Width = block.Width / 1.5;
                    }
                }
                foreach (TextBlock block in (BidsAndOffersList.Header as StackPanel).Children) {
                    block.FontSize = block.FontSize / 1.5;
                    block.Width = block.Width / 1.5;
                }
                foreach (StackPanel panel in BidsAndOffersList.Items) {
                    foreach (TextBlock block in panel.Children) {
                        block.FontSize = block.FontSize / 1.5;
                        block.Width = block.Width / 1.5;
                    }
                }
            } else if (PagePane.ActualWidth > 1400 && screenState != ScreenState.JustLoaded && screenState != ScreenState.BigDesktop) {
                screenState = ScreenState.BigDesktop;
                foreach (TextBlock block in (InventoryList.Header as StackPanel).Children) {
                    block.FontSize = block.FontSize * 1.5;
                    block.Width = block.Width * 1.5;
                }
                foreach (StackPanel panel in InventoryList.Items) {
                    foreach (TextBlock block in panel.Children) {
                        block.FontSize = block.FontSize * 1.5;
                        block.Width = block.Width * 1.5;
                    }
                }
                foreach (TextBlock block in (TradeList.Header as StackPanel).Children) {
                    block.FontSize = block.FontSize * 1.5;
                    block.Width = block.Width * 1.5;
                }
                foreach (StackPanel panel in TradeList.Items) {
                    foreach (TextBlock block in panel.Children) {
                        block.FontSize = block.FontSize * 1.5;
                        block.Width = block.Width * 1.5;
                    }
                }
                foreach (TextBlock block in (BidsAndOffersList.Header as StackPanel).Children) {
                    block.FontSize = block.FontSize * 1.5;
                    block.Width = block.Width * 1.5;
                }
                foreach (StackPanel panel in BidsAndOffersList.Items) {
                    foreach (TextBlock block in panel.Children) {
                        block.FontSize = block.FontSize * 1.5;
                        block.Width = block.Width * 1.5;
                    }
                }
            }
        }

        ScreenState screenState = ScreenState.JustLoaded;

        private void InventoryItemPressed(object sender, TappedRoutedEventArgs e) {
            string StockName = (((sender as ListView).SelectedItem as StackPanel).Children[0] as TextBlock).Text;
            this.Frame.Navigate(typeof(Pages.User.StockPage), StockName);
        }
    }
    enum ScreenState { SmallDesktop, BigDesktop, JustLoaded }
}

