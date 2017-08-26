using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
    public sealed partial class AdvanceSearch : Page {

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
        public AdvanceSearch() {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            PriceOpporator.Items.Add(">");
            PriceOpporator.Items.Add("<");
            DailyMovementOpporator.Items.Add(">");
            DailyMovementOpporator.Items.Add("<");
        }

        private void SearchButtonClicked(object sender, RoutedEventArgs e) {
            bool CheckBoxSelected = false;
            string command = "SELECT StockName, FullName, CurrentPrice, OpeningPriceToday FROM Stock WHERE";
            if (SearchBoxCheckBox.IsChecked == true && SearchBox.Text.Length > 0) {
                command += " (FullName LIKE '%" + SearchBox.Text + "%' OR StockName LIKE '%" + SearchBox.Text + "%') AND";
                CheckBoxSelected = true;
            }
            double DailyMovementDouble;
            if (DailyMovementCheckBox.IsChecked == true && DailyMovementOpporator.SelectedItem != null && double.TryParse(DailyMovementValue.Text, out DailyMovementDouble)) {
                command += " (CurrentPrice - OpeningPriceToday)/OpeningPriceToday " + DailyMovementOpporator.SelectedItem + " " + DailyMovementDouble + " AND";
                CheckBoxSelected = true;
            }
            double PriceDouble;
            if (PriceCheckBox.IsChecked == true && PriceOpporator.SelectedItem != null && double.TryParse(PriceValue.Text, out PriceDouble)) {
                command += " CurrentPrice " + PriceOpporator.SelectedItem + " " + PriceDouble + " AND";
                CheckBoxSelected = true;
            }
            if (CheckBoxSelected) {
                command = command.Remove(command.Length - 4, 4);
                this.Frame.Navigate(typeof(Pages.User.SearchResults), new SearchParams(command, true));
            }
        }
    }
}
