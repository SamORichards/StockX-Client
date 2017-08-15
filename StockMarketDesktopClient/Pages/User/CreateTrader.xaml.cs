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
            public List<Trigger> Triggers = new List<Trigger>();
            public List<Action> Actions = new List<Action>();
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
            trader.Triggers.Add(new Trigger());
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
            trader.Triggers[0].Target = (sender as ComboBox).SelectedItem as string;
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
                trader.Triggers[0].Operator = MathOperator.Greater;
            } else {
                trader.Triggers[0].Operator = MathOperator.Less;
            }
            trader.Actions.Clear();
            trader.Actions.Add(new Action());
            panel.Children.Add(Helper.CreateTextBlock("  $", TextAlignment.Left, 0, 22));
            TextBox priceBox = new TextBox();
            priceBox.PlaceholderText = "Price";
            priceBox.VerticalAlignment = VerticalAlignment.Top;
            panel.Children.Add(priceBox);
            trader.Triggers[0].box = priceBox;
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
            trader.Actions[0].box = QuantityBox;
            panel.Children.Add(Helper.CreateTextBlock("", TextAlignment.Left, 0, 22, 10));
            panel.Children.Add(QuantityBox);
            ComboBox box = new ComboBox();
            foreach (string s in StockName) {
                box.Items.Add(s);
            }
            if ((sender as ComboBox).SelectedItem as string == "Buy") {
                trader.Actions[0].BuyOrSell = BuySell.Buy;
            } else {
                trader.Actions[0].BuyOrSell = BuySell.Sell;
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
            trader.Actions[0].Target = (sender as ComboBox).SelectedItem as string;
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
            foreach (Trigger t in trader.Triggers) {
                if (!double.TryParse(t.box.Text, out t.Value)) {
                    PricesParsable = false;
                }
            }
            foreach (Action a in trader.Actions) {
                if (!int.TryParse(a.box.Text, out a.Quantity)) {
                    PricesParsable = false;
                }
            }
            if (PricesParsable) {
                string Command = "";
                foreach (Trigger t in trader.Triggers) {
                    Command += t.Target + "|" + (int)t.Operator + "|" + t.Value + "*";
                }
                Command += "#";
                foreach (Action a in trader.Actions) {
                    Command += a.Target + "|" + (int)a.BuyOrSell + "|" + a.Quantity + "*";
                }
                DataBaseHandler.SetData("INSERT INTO AlgoTraders(OwnerID, Command) VALUES(" + DataBaseHandler.UserID + ", '" + Command + "')");
                this.Frame.Navigate(typeof(Pages.User.AlgoTrading));
            }
        }
    }
}
