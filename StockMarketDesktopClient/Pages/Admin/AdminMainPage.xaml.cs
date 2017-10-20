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

namespace StockMarketDesktopClient.Pages.Admin {
    public sealed partial class AdminMainPage : Page {
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
        public AdminMainPage() {
            this.InitializeComponent();
        }


    }
}
