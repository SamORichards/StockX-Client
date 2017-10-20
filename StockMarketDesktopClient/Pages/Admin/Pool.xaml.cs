using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Pomelo.Data.MySql;
using StockMarketDesktopClient.Scripts;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StockMarketDesktopClient.Pages.Admin {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Pool : Page {
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
        #endregion

        public class PieChartData {
            public string Name { get; set; }
            public double Value { get; set; }
            public PieChartData(string Name) {
                this.Name = Name;
            }
        }

        public ObservableCollection<PieChartData> Demands { get; set; }

        public Pool() {
            this.InitializeComponent();
            this.Demands = new ObservableCollection<PieChartData>();
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT DISTINCT StockName FROM Pool");
            while (reader.Read()) {
                Demands.Add(new PieChartData((string)reader["StockName"]));
            }
            foreach (PieChartData pcd in Demands) {
                pcd.Value = DataBaseHandler.GetCount("SELECT COUNT(*) FROM Pool WHERE StockName = '" + pcd.Name + "'");
            }
            SfChart chart = new SfChart();
            chart.Header = "Stocks In Pool";
            CategoryAxis primaryCategoryAxis = new CategoryAxis();
            primaryCategoryAxis.Header = "Stocks";
            chart.PrimaryAxis = primaryCategoryAxis;
            NumericalAxis secondaryNumericalAxis = new NumericalAxis();
            secondaryNumericalAxis.Header = "Percentage in Pool";
            chart.SecondaryAxis = secondaryNumericalAxis;
            PieSeries series1 = new PieSeries();
            series1.ItemsSource = this.Demands;
            series1.XBindingPath = "Name";
            series1.YBindingPath = "Value";
            chart.Series.Add(series1);
            ChartLegend legend = new ChartLegend();
            chart.Legend = legend;
            MainThing.Children.Add(chart);
        }
    }
}
