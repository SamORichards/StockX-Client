using Pomelo.Data.MySql;
using StockMarketDesktopClient.Scripts;
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

namespace StockMarketDesktopClient.Pages.Admin {
    public sealed partial class StockListPage : Page {
        #region HBMenu
        private void HamburgerButton_Click(object sender, RoutedEventArgs e) {
            if (HB_Menu.IsPaneOpen) {
                HB_Menu.IsPaneOpen = false;
            } else {
                HB_Menu.IsPaneOpen = true;
            }
        }
        private void MainMenuClick(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(Pages.Admin.AdminMainPage));
        }
        private void PoolClick(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(Pages.Admin.Pool));
        }
        private void PersonalTradingClick(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(Pages.FeaturedStock));
        }
        private void StockClick(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(Pages.Admin.StockListPage));
        }
        private void AlgoClick(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(Pages.Admin.AlgoTraderManager));
        }
        #endregion
        public StockListPage() {
            this.InitializeComponent();
            LoadValues();
        }
        public void LoadValues() {
            while (SearchResultList.Items.Count > 0) {
                SearchResultList.Items.RemoveAt(0);
            }
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT StockName, FullName, CurrentPrice FROM Stock");
            while (reader.Read()) {
                string Symbol = (string)reader["StockName"];
                string FullName = (string)reader["FullName"];
                double Price = (double)reader["CurrentPrice"];
                StackPanel panel = new StackPanel();
                panel.Orientation = Orientation.Horizontal;
                panel.Children.Add(Helper.CreateTextBlock(Symbol, TextAlignment.Left, 100, 20));
                panel.Children.Add(Helper.CreateTextBlock(FullName, TextAlignment.Left, 350, 20));
                panel.Children.Add(Helper.CreateTextBlock("$" + Math.Round(Price, 4), TextAlignment.Left, 150, 20));
                Button EditButton = new Button();
                EditButton.Content = "Edit";
                EditButton.Width = 60;
                EditButton.Height = 40;
                EditButton.Click += (o, i) => this.Frame.Navigate(typeof(Pages.Admin.StockEditorPage), Symbol);
                Button DeleteButton = new Button();
                DeleteButton.FontFamily = new FontFamily("Segoe MDL2 Assets");
                DeleteButton.Content = "";
                DeleteButton.Click += (o, i) => DeleteStockButtonClick(Symbol);
                DeleteButton.Width = 40;
                DeleteButton.Height = 40;
                panel.Children.Add(EditButton);
                panel.Children.Add(DeleteButton);
                SearchResultList.Items.Add(panel);
            }
        }

        private void DeleteStockButtonClick(string StockName) {
            DataBaseHandler.SetData("DELETE FROM Stock Where StockName = '" + StockName + "'");
            DataBaseHandler.SetData("DELETE FROM Inventories Where StockName = '" + StockName + "'");
            DataBaseHandler.SetData("DELETE FROM Trades Where StockName = '" + StockName + "'");
            DataBaseHandler.SetData("DELETE FROM WatchList Where StockName = '" + StockName + "'");
            DataBaseHandler.SetData("DELETE FROM Pool Where StockName = '" + StockName + "'");
            DataBaseHandler.SetData("DELETE FROM PricingHistory Where StockName = '" + StockName + "'");
            LoadValues();
        }

        private void CreateButtonClick(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(Pages.Admin.StockEditorPage), "");
        }
    }
}
