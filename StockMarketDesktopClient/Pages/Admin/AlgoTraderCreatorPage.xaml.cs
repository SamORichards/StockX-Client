using Pomelo.Data.MySql;
using StockMarketDesktopClient.Scripts;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StockMarketDesktopClient.Pages.Admin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AlgoTraderCreatorPage : Page
    {
        public AlgoTraderCreatorPage()
        {
            this.InitializeComponent();
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT FullName FROM Stock");
            while (reader.Read()) {
                StockNameBox.Items.Add((string)reader["FullName"]);
            }
        }

        private void CreateButtonClicked(object sender, RoutedEventArgs e) {
            string email = "AlgoTrader" + (string)StockNameBox.SelectedItem + "@" + 100 + ".com";
            int UserId = DataBaseHandler.GetCount(string.Format("INSERT INTO Users(NickName, Email, Password, Balance, Admin, LMM) VALUES ('{0}', '{1}', '{2}', {3}, {4}, {5}); SELECT LAST_INSERT_ID();", "AlgoTrader", email, "Password", 1000000, 0, 0.20f));
            DataBaseHandler.SetData(string.Format("INSERT INTO AlgoTrader(Target, UserID, ShortRequirement, LongRequirement, MinAmount, MaxAmount, Aggresion) VALUES ('{0}', {1}, {2}, {3}, {4}, {5}, {6})", (string)StockNameBox.SelectedItem, UserId, ShortRequirementBox.Text, LongRequirementBox, MinAmountBox, MaxAmountBox, AggresionBox));
            this.Frame.Navigate(typeof(Pages.Admin.AlgoTraderManager));
        }
    }
}
