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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StockMarketDesktopClient.Pages.User {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateTrader : Page {
        StackPanel panel = new StackPanel();
        public CreateTrader() {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            panel.Orientation = Orientation.Horizontal;
            panel.Children.Add(Helper.CreateTextBlock("If", TextAlignment.Left, 0, 22));
            ComboBox box = new ComboBox();
            box.Items.Add("test");
            box.Items.Add("type");
            panel.Children.Add(box);
        }
    }
}
