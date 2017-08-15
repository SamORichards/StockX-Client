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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StockMarketDesktopClient.Pages.User {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AlgoTrading : Page {
        public AlgoTrading() {
            this.InitializeComponent();
        }
        List<string> StockName = new List<string>();

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT FullName FROM Stock");
            ComboBox box = new ComboBox();
            while (reader.Read()) {
                box.Items.Add((string)reader["FullName"]);
                StockName.Add((string)reader["FullName"]);
            }
            box.SelectionChanged += StockChoiceChanged;
            box.Padding = new Thickness(5, 0, 5, 0);
            panel.Children.Add(Helper.CreateTextBlock("If ", TextAlignment.Left, 10, 22));
            panel.Children.Add(box);
        }

        private void StockChoiceChanged(object sender, SelectionChangedEventArgs e) {
            while (panel.Children.Count > 2) {
                panel.Children.RemoveAt(panel.Children.Count - 1);
            }
            panel.Children.Add(Helper.CreateTextBlock(" is ", TextAlignment.Left, 0, 22));
            ComboBox box = new ComboBox();
            box.Items.Add(">");
            box.Items.Add("<");
            box.Padding = new Thickness(10, 0, 10, 0);
            box.SelectionChanged += OpporatorChanged;
            panel.Children.Add(box);
        }

        private void OpporatorChanged(object sender, SelectionChangedEventArgs e) {
            while (panel.Children.Count > 4) {
                panel.Children.RemoveAt(panel.Children.Count - 1);
            }
            panel.Children.Add(Helper.CreateTextBlock(" $", TextAlignment.Left, 0, 22));
            TextBox priceBox = new TextBox();
            priceBox.PlaceholderText = "Price";
            priceBox.VerticalAlignment = VerticalAlignment.Top;
            panel.Children.Add(priceBox);
            panel.Children.Add(Helper.CreateTextBlock(" then ", TextAlignment.Left, 0, 22));
            ComboBox box = new ComboBox();
            box.Padding = new Thickness(5, 0, 5, 0);
            box.Items.Add("Buy");
            box.Items.Add("Sell");
            box.SelectionChanged += BuyOrSellChanged;
            panel.Children.Add(box);
        }


        private void BuyOrSellChanged(object sender, SelectionChangedEventArgs e) {
            while (panel.Children.Count > 8) {
                panel.Children.RemoveAt(panel.Children.Count - 1);
            }
            ComboBox box = new ComboBox();
            foreach (string s in StockName) {
                box.Items.Add(s);
            }
            box.Padding = new Thickness(5, 0, 0, 0);
            box.SelectionChanged += StockName2Changed;
            box.VerticalAlignment = VerticalAlignment.Top;
            panel.Children.Add(box);
        }

        private void StockName2Changed(object sender, SelectionChangedEventArgs e) {
            while (panel.Children.Count > 9) {
                panel.Children.RemoveAt(panel.Children.Count - 1);
            }
            Button b = new Button();
            b.Content = "Create Trader";
            b.FontSize = 22;
            b.Padding = new Thickness(5, 0, 5, 0);
            b.VerticalAlignment = VerticalAlignment.Top;
            b.Click += CreateTraderClicked;
            panel.Children.Add(b);
        }

        private void CreateTraderClicked(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
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

