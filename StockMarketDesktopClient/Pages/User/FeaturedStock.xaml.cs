using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using StockMarketDesktopClient.Scripts;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StockMarketDesktopClient.Pages {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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

        private void SearchClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Pages.User.SearchResults), new Pages.User.SearchParams(SearchBox.Text, false));
        }
    }
#endregion
}
