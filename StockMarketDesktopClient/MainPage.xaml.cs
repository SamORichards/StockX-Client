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


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StockMarketDesktopClient {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page {
        public MainPage() {
            this.InitializeComponent();
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e) {
            DataBaseHandler.UserID = 1;
            if (ValidEmail(EmailBox.Text) && ValidUsernamePassword(PasswordBox.Password)) {
                OnlineConnector("Login", "", PasswordBox.Password, EmailBox.Text);
            }
        }

        bool ValidUsernamePassword(string user) {
            bool isValid = true;
            if (string.IsNullOrEmpty(user)) {
                return false;
            }
            foreach (char c in user) {
                if (!char.IsLetter(c) && !char.IsDigit(c)) {
                    isValid = false;
                }
            }
            return isValid;
        }

        bool ValidEmail(string email) {
            bool hasAt = false;
            bool hasDotAtEnd = false;
            if (string.IsNullOrEmpty(email)) {
                return false;
            }
            if (email[email.Length - 1] == '.') {
                return false;
            }
            foreach (char c in email) {
                if (c == '@') {
                    if (!hasAt) {
                        hasAt = true;
                    } else {
                        return false;
                    }
                } else if (c == '.') {
                    if (hasAt) {
                        hasDotAtEnd = true;
                    }
                }
            }
            if (hasAt && hasDotAtEnd) {
                return true;
            }
            return false;
        }

        void OnlineConnector(string Act, string formNick, string formPassword, string email) {
            if (Act == "Login") {
                 MySqlDataReader reader = DataBaseHandler.GetData("SELECT * FROM Users WHERE Email = '" + email + "'");
                string Nickname = "";
                int UserId = 0;
                string Password = "";
                while (reader.Read()) {
                    Nickname = (string)reader["Nickname"];
                    UserId = (int)reader["ID"];
                    Password = (string)reader["Password"];
                }
                if (Password == formPassword) {
                    DataBaseHandler.Nickname = Nickname;
                    DataBaseHandler.UserID = UserId;
                    this.Frame.Navigate(typeof(Pages.FeaturedStock));                    
                }
            }

        }
    }
}
