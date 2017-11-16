using Pomelo.Data.MySql;
using StockMarketDesktopClient.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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

        private async void CreateButtonClicked(object sender, RoutedEventArgs e) {
            uint MinAmount = 0;
            uint MaxAmount = 0;
            double ShortRequirement = 0;
            double LongRequirement = 0;
            double Aggresion = 0;
            string email = "AlgoTrader" + (string)StockNameBox.SelectedItem + "@" + 100 + ".com";
            int UserId = DataBaseHandler.GetCount(string.Format("INSERT INTO Users(NickName, Email, Password, Balance, Admin, LMM) VALUES ('{0}', '{1}', '{2}', {3}, {4}, {5}); SELECT LAST_INSERT_ID();", "AlgoTrader", email, "Password", 1000000, 0, 0.20f));
            if ((string)StockNameBox.SelectedItem == "") {
                MessageDialog message = new MessageDialog("No Stock Selected");
                await message.ShowAsync();
                return;
            }
            if (double.TryParse(ShortRequirementBox.Text, out ShortRequirement)) {
                MessageDialog message = new MessageDialog("Short Requirement needs to be a number");
                await message.ShowAsync();
                return;
            }
            if (ShortRequirement > 1.1f && ShortRequirement < 2.1f) {
                MessageDialog message = new MessageDialog("Short Requirement needs to be between 1.1 and 2.1");
                await message.ShowAsync();
                return;
            }
            if (double.TryParse(ShortRequirementBox.Text, out LongRequirement)) {
                MessageDialog message = new MessageDialog("Long Requirement needs to be a number");
                await message.ShowAsync();
                return;
            }
            if (LongRequirement > 1.1f && LongRequirement < 2.1f) {
                MessageDialog message = new MessageDialog("Long Requirement needs to be between 1.9 and 2.9");
                await message.ShowAsync();
                return;
            }
            if (uint.TryParse(MinAmountBox.Text, out MinAmount)) { 
                MessageDialog message = new MessageDialog("Minimum Amount needs to be a integer");
                await message.ShowAsync();
                return;
            }
            if (uint.TryParse(MinAmountBox.Text, out MaxAmount)) {
                MessageDialog message = new MessageDialog("Maximum Amount needs to be a integer");
                await message.ShowAsync();
                return;
            }
            if (MaxAmount < MinAmount) {
                MessageDialog message = new MessageDialog("Minimum Amount needs smaller than Max Amount");
                await message.ShowAsync();
                return;
            }
            if (double.TryParse(AggresionBox.Text, out Aggresion)) {
                MessageDialog message = new MessageDialog("Aggresion needs to be a number");
                await message.ShowAsync();
                return;
            }
            if (LongRequirement > 1.1f && LongRequirement < 2.1f) {
                MessageDialog message = new MessageDialog("Aggresion needs to be between 0 and 1");
                await message.ShowAsync();
                return;
            }
            DataBaseHandler.SetData(string.Format("INSERT INTO AlgoTrader(Target, UserID, ShortRequirement, LongRequirement, MinAmount, MaxAmount, Aggresion) VALUES ('{0}', {1}, {2}, {3}, {4}, {5}, {6})", (string)StockNameBox.SelectedItem, UserId, ShortRequirement, LongRequirement, MinAmount, MaxAmount, Aggresion));
            this.Frame.Navigate(typeof(Pages.Admin.AlgoTraderManager));
        }
    }
}
