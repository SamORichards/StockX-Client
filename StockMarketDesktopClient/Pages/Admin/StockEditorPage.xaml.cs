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
using Windows.UI.Popups;

namespace StockMarketDesktopClient.Pages.Admin {
    public sealed partial class StockEditorPage : Page {
        bool NewStock = true;
        string OriginalStockName = "";

        public StockEditorPage() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            if ((string)e.Parameter != "") {
                NewStock = false;
                LoadValues((string)e.Parameter);
                QuantityPane.Children.Clear();
            }
        }

        private void LoadValues(string Symbol) {
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT FullName, Description, CurrentPrice FROM Stock WHERE StockName = '" + Symbol + "'");
            string FullName = "", Description = "";
            float CurrentPrice = 0;
            while (reader.Read()) {
                FullName = (string)reader["FullName"];
                CurrentPrice = (float)reader["CurrentPrice"];
            }
            SymbolBlock.Text = Symbol;
            PriceBlock.Text = CurrentPrice.ToString();
            DescriptionBlock.Text = Description;
            OriginalStockName = Symbol;
        }

        private async void CreateButtonClicked(object sender, RoutedEventArgs e) {
            string Symbol = SymbolBlock.Text;
            string FullName = NameBlock.Text;
            int Taken = DataBaseHandler.GetCount("SELECT COUNT(*) FROM Stock WHERE StockName = '" + Symbol + "'");
            string Description = DescriptionBlock.Text;
            if (Symbol.Length != 6) {
                MessageDialog message = new MessageDialog("Symbol must be exactly 6 letters long");
                await message.ShowAsync();
                return;
            }
            if (Taken == 1) {
                MessageDialog message = new MessageDialog("Symbol is taken, try another name");
                await message.ShowAsync();
                return;
            }
            if (FullName.Length == 0) {
                MessageDialog message = new MessageDialog("Name is required");
                await message.ShowAsync();
                return;
            }
            if (Description.Length == 0) {
                MessageDialog message = new MessageDialog("Description is required");
                await message.ShowAsync();
                return;
            }
            float StartingPrice;
            int IPOQuantity;
            if (!(float.TryParse(PriceBlock.Text, out StartingPrice) && StartingPrice > 0)) {
                MessageDialog message = new MessageDialog("Price is not valid");
                await message.ShowAsync();
                return;
            }
            if ((!(int.TryParse(QuantityBlock.Text, out IPOQuantity) && IPOQuantity > 0) || !NewStock)) {
                MessageDialog message = new MessageDialog("IPO Quantity is not valid");
                await message.ShowAsync();
                return;
            }
            if (!NewStock) {
                DataBaseHandler.SetData("DELETE FROM Stock WHERE StockName = '" + OriginalStockName + "'");
                DataBaseHandler.SetData("UPDATE Inventories SET StockName = + '" + Symbol+"' Where StockName = '" + OriginalStockName + "'");
                DataBaseHandler.SetData("UPDATE Trades SET StockName = + '" + Symbol + "' Where StockName = '" + OriginalStockName + "'");
                DataBaseHandler.SetData("UPDATE WatchList SET StockName = + '" + Symbol + "' Where StockName = '" + OriginalStockName + "'");
                DataBaseHandler.SetData("UPDATE Pool SET StockName = + '" + Symbol + "' Where StockName = '" + OriginalStockName + "'");
                DataBaseHandler.SetData("UPDATE PricingHistory SET StockName = + '" + Symbol + "' Where StockName = '" + OriginalStockName + "'");
            } else {
                DataBaseHandler.SetData(string.Format("INSERT INTO Inventories(UserID, StockName, Quantity, LastTradedPrice) VALUES ({0}, '{1}', {2}, {3})", IPOQuantity, Symbol, 1, StartingPrice));
            }
            DataBaseHandler.SetData(string.Format("INSERT INTO Stock(StockName, FullName, Description, CurrentPrice, OpeningPriceToday, HighToday, LowToday, VolumeTraded) VALUES ('{0}', '{1}', '{2}', {3}, {4}, {5}, {6}, {7})", Symbol, FullName, Description, StartingPrice, StartingPrice, 0f, float.MaxValue, 0));
            this.Frame.Navigate(typeof(Pages.Admin.StockListPage));
        }
    }
}
