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
using Pomelo.Data.MySql;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StockMarketDesktopClient.Pages.User {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateTrader : Page {
        public CreateTrader() {
            this.InitializeComponent();
        }
        List<string> StockName = new List<string>();
        class Trader {
            public Trigger trigger;
            public Action action;
        }
        class Trigger {
            public string Target;
            public MathOperator Operator;
            public double Value;
            public TextBox box;
        }
        class Action {
            public string Target;
            public BuySell BuyOrSell;
            public int Quantity;
            public TextBox box;
        }
        Trader trader = new Trader();
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT FullName FROM Stock");
            ComboBox box = new ComboBox();
            trader.trigger = new Trigger();
            while (reader.Read()) {
                box.Items.Add((string)reader["FullName"]);
                StockName.Add((string)reader["FullName"]);
            }
            box.SelectionChanged += StockChoiceChanged;
            panel.Children.Add(Helper.CreateTextBlock("If ", TextAlignment.Left, 25, 22, 10));
            panel.Children.Add(box);
        }

        private void StockChoiceChanged(object sender, SelectionChangedEventArgs e) {
            while (panel.Children.Count > 2) {
                panel.Children.RemoveAt(panel.Children.Count - 1);
            }
            panel.Children.Add(Helper.CreateTextBlock(" is", TextAlignment.Left, 0, 22, 10));
            trader.trigger.Target = (sender as ComboBox).SelectedItem as string;
            ComboBox box = new ComboBox();
            box.Items.Add(">");
            box.Items.Add("<");
            box.SelectionChanged += OpporatorChanged;
            panel.Children.Add(box);
        }

        private void OpporatorChanged(object sender, SelectionChangedEventArgs e) {
            while (panel.Children.Count > 4) {
                panel.Children.RemoveAt(panel.Children.Count - 1);
            }
            if ((sender as ComboBox).SelectedItem as string == ">") {
                trader.trigger.Operator = MathOperator.Greater;
            } else {
                trader.trigger.Operator = MathOperator.Less;
            }
            trader.action = new Action();
            panel.Children.Add(Helper.CreateTextBlock("  $", TextAlignment.Left, 0, 22));
            TextBox priceBox = new TextBox();
            priceBox.PlaceholderText = "Price";
            priceBox.VerticalAlignment = VerticalAlignment.Top;
            panel.Children.Add(priceBox);
            trader.trigger.box = priceBox;
            panel.Children.Add(Helper.CreateTextBlock(" then ", TextAlignment.Left, 0, 22, 10));
            ComboBox box = new ComboBox();
            box.Items.Add("Buy");
            box.Items.Add("Sell");
            box.SelectionChanged += BuyOrSellChanged;
            panel.Children.Add(box);
        }

        private void BuyOrSellChanged(object sender, SelectionChangedEventArgs e) {
            while (panel.Children.Count > 8) {
                panel.Children.RemoveAt(panel.Children.Count - 1);
            }
            TextBox QuantityBox = new TextBox();
            QuantityBox.PlaceholderText = "Quantity";
            QuantityBox.VerticalAlignment = VerticalAlignment.Top;
            trader.action.box = QuantityBox;
            panel.Children.Add(Helper.CreateTextBlock("", TextAlignment.Left, 0, 22, 10));
            panel.Children.Add(QuantityBox);
            ComboBox box = new ComboBox();
            foreach (string s in StockName) {
                box.Items.Add(s);
            }
            if ((sender as ComboBox).SelectedItem as string == "Buy") {
                trader.action.BuyOrSell = BuySell.Buy;
            } else {
                trader.action.BuyOrSell = BuySell.Sell;
            }
            box.SelectionChanged += StockName2Changed;
            box.VerticalAlignment = VerticalAlignment.Top;
            panel.Children.Add(Helper.CreateTextBlock("", TextAlignment.Left, 0, 22, 10));
            panel.Children.Add(box);
        }

        private void StockName2Changed(object sender, SelectionChangedEventArgs e) {
            while (panel.Children.Count > 12) {
                panel.Children.RemoveAt(panel.Children.Count - 1);
            }
            trader.action.Target = (sender as ComboBox).SelectedItem as string;
            Button b = new Button();
            panel.Children.Add(Helper.CreateTextBlock("", TextAlignment.Left, 0, 22, 10));
            b.Content = "Create Trader";
            b.FontSize = 22;
            b.VerticalAlignment = VerticalAlignment.Top;
            b.Click += CreateTraderClicked;
            panel.Children.Add(b);
        }

        private void CreateTraderClicked(object sender, RoutedEventArgs e) {
            bool PricesParsable = true;
                if (!double.TryParse(trader.trigger.box.Text, out trader.trigger.Value)) {
                    PricesParsable = false;
                }
                if (!int.TryParse(trader.action.box.Text, out trader.action.Quantity)) {
                    PricesParsable = false;
                }
            if (PricesParsable) {
                DataBaseHandler.SetData(string.Format("INSERT INTO UserAlgoTraders (OwnerID, TTarget, TOpperation, TValue, ATarget, ABuyOrSell, AQuantity) VALUES({0}, '{1}', {2}, {3}, '{4}', {5}, {6})", DataBaseHandler.UserID, trader.trigger.Target, (int)trader.trigger.Operator, trader.trigger.Value, trader.action.Target, (int)trader.action.BuyOrSell, trader.action.Quantity));
                this.Frame.Navigate(typeof(Pages.User.AlgoTrading));
            }
        }
    }
}
