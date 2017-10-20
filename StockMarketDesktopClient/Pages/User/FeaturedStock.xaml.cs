using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using StockMarketDesktopClient.Scripts;
using Pomelo.Data.MySql;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace StockMarketDesktopClient.Pages {
    public sealed partial class FeaturedStock : Page {
        public FeaturedStock() {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            if (Helper.FirstLoad) {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                SystemNavigationManager.GetForCurrentView().BackRequested += (s, ev) => {
                    if (Frame.CanGoBack) {
                        Frame.GoBack();
                        ev.Handled = true;
                    }
                };
                Helper.FirstLoad = false;
            }
            if (DataBaseHandler.GetCount("SELECT SUM(Admin) FROM Users WHERE ID = " + DataBaseHandler.UserID) == 0) {
                AdminButton.IsEnabled = false;
                AdminButton.Background = new SolidColorBrush(Colors.White);
                AdminButton.Content = "";
            }
            LoadBiggestRiser();
            LoadBiggestFallers();
        }

        private void LoadBiggestRiser() {
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT StockName, CurrentPrice, OpeningPriceToday FROM Stock ORDER BY CurrentPrice - OpeningPriceToday DESC LIMIT 4");
            while (reader.Read()) {
                StackPanel panel = new StackPanel();
                panel.Orientation = Orientation.Horizontal;
                panel.HorizontalAlignment = HorizontalAlignment.Left;
                double CurrentPrice = (double)reader["CurrentPrice"];
                panel.Children.Add(Helper.CreateTextBlock((string)reader["StockName"], TextAlignment.Left, 100, 18));
                panel.Children.Add(Helper.CreateTextBlock(Math.Round(CurrentPrice, 4).ToString(), TextAlignment.Left, 100, 18));
                panel.Children.Add(Helper.CreateTextBlock(Math.Round(CurrentPrice - (double)reader["OpeningPriceToday"], 4).ToString(), TextAlignment.Left, 100, 18));
                BiggestRisers.Items.Add(panel);
            }
        }
        private void LoadBiggestFallers() {
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT StockName, CurrentPrice, OpeningPriceToday FROM Stock ORDER BY CurrentPrice - OpeningPriceToday ASC LIMIT 4");
            while (reader.Read()) {
                StackPanel panel = new StackPanel();
                panel.Orientation = Orientation.Horizontal;
                panel.HorizontalAlignment = HorizontalAlignment.Left;
                double CurrentPrice = (double)reader["CurrentPrice"];
                panel.Children.Add(Helper.CreateTextBlock((string)reader["StockName"], TextAlignment.Left, 100, 18));
                panel.Children.Add(Helper.CreateTextBlock(Math.Round(CurrentPrice, 4).ToString(), TextAlignment.Left, 100, 18));
                panel.Children.Add(Helper.CreateTextBlock(Math.Round(CurrentPrice - (double)reader["OpeningPriceToday"], 4).ToString(), TextAlignment.Left, 100, 18));
                BiggestFallers.Items.Add(panel);
            }
        }
        private void StockTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
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

        private void SearchClick(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(Pages.User.SearchResults), new Pages.User.SearchParams(SearchBox.Text, false));
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(Pages.Admin.AdminMainPage));
        }
    }
    #endregion
}
