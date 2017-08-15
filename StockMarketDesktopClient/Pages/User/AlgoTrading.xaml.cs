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
    /// 
    enum MathOperator { Greater, Less }
    enum BuySell { Buy, Sell }
    public sealed partial class AlgoTrading : Page {
        public AlgoTrading() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            LoadTraders();
        }

        void LoadTraders() {
            TradersList.Items.Clear();
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT * FROM AlgoTraders WHERE OwnerID = " + DataBaseHandler.UserID);
            while (reader.Read()) {
                StackPanel panel = new StackPanel();
                panel.Orientation = Orientation.Horizontal;
                panel.Children.Add(Helper.CreateTextBlock(((int)reader["ID"]).ToString(), TextAlignment.Left, 25, 22));
                Button b = new Button();
                b.Content = "Remove";
                b.Click += (s, ev) => { DataBaseHandler.SetData("DELETE FROM AlgoTraders WHERE ID = " + reader["ID"]); LoadTraders(); };
                panel.Children.Add(b);
                TradersList.Items.Add(panel);
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

        private void CreateTraderButton(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(Pages.User.CreateTrader));
        }
    }
    #endregion
}

