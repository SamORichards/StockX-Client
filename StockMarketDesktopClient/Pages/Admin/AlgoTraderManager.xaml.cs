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
    public sealed partial class AlgoTraderManager : Page {
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
        public AlgoTraderManager() {
            this.InitializeComponent();
            LoadValues();
        }
        public void LoadValues() {
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT AlgoTrader.ID, AlgoTrader.Target, AlgoTrader.UserID, AlgoTrader.ShortRequirement, AlgoTrader.LongRequirement, AlgoTrader.MinAmount, AlgoTrader.MaxAmount, AlgoTrader.Aggresion, Users.Balance FROM AlgoTrader, Users WHERE AlgoTrader.UserID = Users.ID");
            while (reader.Read()) {
                int ID = (int)reader["ID"];
                string Target = (string)reader["Target"];
                double ShortRequirement = (double)reader["ShortRequirement"];
                double LongRequirement = (double)reader["LongRequirement"];
                int MinAmount = (int)reader["MinAmount"];
                int MaxAmount = (int)reader["MaxAmount"];
                double Aggresion = (double)reader["Aggresion"];
                double Price = (double)reader["Balance"];
                StackPanel panel = new StackPanel();
                panel.Orientation = Orientation.Horizontal;
                panel.Children.Add(Helper.CreateTextBlock(ID.ToString(), TextAlignment.Left, 35, 20));
                panel.Children.Add(Helper.CreateTextBlock(Target, TextAlignment.Left, 100, 20));
                panel.Children.Add(Helper.CreateTextBlock(ShortRequirement.ToString(), TextAlignment.Left, 200, 20));
                panel.Children.Add(Helper.CreateTextBlock(LongRequirement.ToString(), TextAlignment.Left, 200, 20));
                panel.Children.Add(Helper.CreateTextBlock(MinAmount.ToString(), TextAlignment.Left, 150, 20));
                panel.Children.Add(Helper.CreateTextBlock(MaxAmount.ToString(), TextAlignment.Left, 150, 20));
                panel.Children.Add(Helper.CreateTextBlock(Math.Round(Aggresion, 5).ToString(), TextAlignment.Left, 150, 20));
                panel.Children.Add(Helper.CreateTextBlock(Price.ToString(), TextAlignment.Left, 100, 20));
                Button DeleteButton = new Button();
                DeleteButton.FontFamily = new FontFamily("Segoe MDL2 Assets");
                DeleteButton.Content = "";
                DeleteButton.Click += (o, i) => DeleteStockButtonClick(ID);
                DeleteButton.Width = 40;
                DeleteButton.Height = 40;
                panel.Children.Add(DeleteButton);
                SearchResultList.Items.Add(panel);
            }
        }

        void DeleteStockButtonClick(int ID) {
            DataBaseHandler.SetData("DELETE FROM AlgoTrader WHERE ID = " + ID);
            this.Frame.Navigate(typeof(Pages.Admin.AlgoTraderManager));
        }

        private void CreateButtonClicked(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(Pages.Admin.AlgoTraderCreatorPage));
        }
    }
}
