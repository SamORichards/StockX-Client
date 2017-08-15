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
using StockMarketDesktopClient.Scripts;
using Pomelo.Data.MySql;
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StockMarketDesktopClient.Pages.User {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    enum BidOffer { bid, offer }
    public sealed partial class BuySellPage : Page {
        string StockName;
        BidOffer bidOffer;
        public BuySellPage() {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            StockName = (e.Parameter as string).Split('/')[0];
            bidOffer = (BidOffer)int.Parse((e.Parameter as string).Split('/')[1]);
            if (bidOffer == BidOffer.offer) {
                BuySellButton.Background = new SolidColorBrush(Colors.Red);
                BuySellButton.Content = "Sell";
            }
            double Balance = DataBaseHandler.GetCountDouble("SELECT SUM(Balance) FROM Users WHERE ID = " + DataBaseHandler.UserID);
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT Price, Quantity FROM Pool WHERE Type = 0 AND User = " + DataBaseHandler.UserID);
            double MoneyInPool = 0;
            while (reader.Read()) {
                MoneyInPool += (double)reader["Price"] * (int)reader["Quantity"];
            }
            double FundsAvailable = Balance - MoneyInPool;
            FundsAvailableBlock.Text = "Funds Available: " + FundsAvailable.ToString().Split('.')[0] + "." + FundsAvailable.ToString().Split('.')[1].Substring(0, 4);
        }

        private void BuySellButtonClicked(object sender, RoutedEventArgs e) {
            int Quantity;
            if (int.TryParse(QuantityBox.Text, out Quantity)) {
                double Price = DataBaseHandler.GetCountDouble("SELECT SUM(CurrentPrice) FROM Stock WHERE StockName = '" + StockName + "'");
                switch (bidOffer) {
                    case BidOffer.bid:
                        double Balance = DataBaseHandler.GetCountDouble("SELECT SUM(Balance) FROM Users WHERE ID = " + DataBaseHandler.UserID);
                        MySqlDataReader reader = DataBaseHandler.GetData("SELECT Price, Quantity FROM Pool WHERE Type = 0 AND User = " + DataBaseHandler.UserID);
                        double MoneyInPool = 0;
                        while (reader.Read()) {
                            MoneyInPool += (double)reader["Price"] * (int)reader["Quantity"];
                        }
                        double FundsAvailable = Balance - MoneyInPool;
                        int CanAfford = (int)(FundsAvailable / Price);
                        if (Quantity > CanAfford) { Quantity = CanAfford; };
                        DataBaseHandler.SetData(string.Format("INSERT INTO Pool (Type, Price, User, StockName, Quantity) VALUES ({0}, {1}, {2}, '{3}', {4})", (int)bidOffer, Price, DataBaseHandler.UserID, StockName, Quantity));
                        this.Frame.Navigate(typeof(Pages.User.Portfolio));
                        break;
                    case BidOffer.offer:
                        int Owned = DataBaseHandler.GetCount("SELECT COUNT(StockID) FROM StocksInCirculation WHERE OwnerID = " + DataBaseHandler.UserID);
                        int AlreadySelling = DataBaseHandler.GetCount("SELECT SUM(Quantity) FROM Pool WHERE Type = 1 AND User = " + DataBaseHandler.UserID);
                        int CanSell = Owned - AlreadySelling;
                        if (Quantity > CanSell) { Quantity = CanSell; }
                        DataBaseHandler.SetData(string.Format("INSERT INTO Pool (Type, Price, User, StockName, Quantity) VALUES ({0}, {1}, {2}, '{3}', {4})", (int)bidOffer, Price, DataBaseHandler.UserID, StockName, Quantity));
                        break;
                }
            } else {
                //TODO: Warning Box


            }
        }
    }
}
