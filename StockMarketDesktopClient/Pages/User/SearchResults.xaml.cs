using Pomelo.Data.MySql;
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
using Windows.UI.Core;
using Windows.UI;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StockMarketDesktopClient.Pages.User {
    /// <summary> 
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchResults : Page {
        public SearchResults() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            Search((string)e.Parameter);
        }



        public void Search(string DataValue) {
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT StockName, FullName, CurrentPrice, OpeningPriceToday FROM Stock WHERE FullName LIKE '%" + DataValue + "%' OR StockName LIKE '%" + DataValue + "%'");
            while (reader.Read()) {
                string Symbol = (string)reader["StockName"];
                string FullName = (string)reader["FullName"];
                double Price = (double)reader["CurrentPrice"];
                double OpeningPrice = (double)reader["OpeningPriceToday"];
                double RealChangeInPrice = Price - OpeningPrice;
                double PercentageChange = RealChangeInPrice / OpeningPrice;
                StackPanel panel = new StackPanel();
                panel.Orientation = Orientation.Horizontal;
                panel.Children.Add(Helper.CreateTextBlock(Symbol, TextAlignment.Left, 100, 20));
                panel.Children.Add(Helper.CreateTextBlock(FullName, TextAlignment.Left, 250, 20));
                panel.Children.Add(Helper.CreateTextBlock("$" + Price.ToString().Split('.')[0] + '.' + Price.ToString().Split('.')[1].Substring(0, 4), TextAlignment.Left, 100, 20));
                TextBlock RealChangeInPriceBlock = Helper.CreateTextBlock(RealChangeInPrice.ToString(), TextAlignment.Left, 100, 20);
                if (RealChangeInPrice.ToString().Contains('.')) {
                    RealChangeInPriceBlock = Helper.CreateTextBlock(RealChangeInPrice.ToString().Substring(0, 6), TextAlignment.Left, 100, 20);
                }
                if (RealChangeInPrice < 0) {
                    RealChangeInPriceBlock.Foreground = new SolidColorBrush(Colors.Red);
                } else {
                    RealChangeInPriceBlock.Foreground = new SolidColorBrush(Colors.Green);
                }
                panel.Children.Add(RealChangeInPriceBlock);
                TextBlock PercentageChangeBlock;
                if (PercentageChange.ToString().Contains('.')) {
                    PercentageChangeBlock = Helper.CreateTextBlock(PercentageChange.ToString().Substring(0, 4) + "%", TextAlignment.Left, 100, 20);
                } else {
                    PercentageChangeBlock = Helper.CreateTextBlock(PercentageChange.ToString() + "%", TextAlignment.Left, 100, 20);
                }
                if (RealChangeInPrice < 0) {
                    PercentageChangeBlock.Foreground = new SolidColorBrush(Colors.Red);
                } else {
                    PercentageChangeBlock.Foreground = new SolidColorBrush(Colors.Green);
                }
                panel.Children.Add(PercentageChangeBlock);
                SearchResultList.Items.Add(panel);
            }
        }

        private void ItemClickedListView(object sender, TappedRoutedEventArgs e) {
            string StockName = (((sender as ListView).SelectedItem as StackPanel).Children[0] as TextBlock).Text;
            this.Frame.Navigate(typeof(Pages.User.StockPage), StockName);
        }
    }
}
 