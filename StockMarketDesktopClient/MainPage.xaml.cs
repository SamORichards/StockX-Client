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
using System.Net.Http;
using Microsoft.Data.Sqlite;
using Microsoft.Data.Sqlite.Internal;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StockMarketDesktopClient {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page {
        private static readonly HttpClient client = new HttpClient();
        string URL = "http://www.lemonjuicestudios.com/Stocks/S_Login.php";
        public MainPage() {
            this.InitializeComponent();
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e) {
            if (ValidEmail(EmailBox.Text) && ValidUsernamePassword(PasswordBox.Password)) {
                OnlineConnectorAsync("Login", "", PasswordBox.Password, EmailBox.Text);
                SqliteConnection con = new SqliteConnection();
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

        async void OnlineConnectorAsync(string Act, string formNick, string formPassword, string email) {
            string tempURL;
            if (Act == "Login") {
                tempURL = URL + "?Email=" + email + "&Pass=" + formPassword + "&Act=" + Act;
            } else {
                tempURL = URL + "?User=" + formNick + "&Pass=" + formPassword + "&Act=" + Act + "&Email=" + email;
            }
            var responseString = await client.GetStringAsync(tempURL);
            //while (!w.isDone) yield return null;

            if (responseString.Length > 7 && responseString.Substring(0, 7) == "Correct") {
                //Login
            }
            if (responseString == "Wrong") {
                //Wrong Password
            }
            if (responseString == "No User") {
                //Wrong Email
            }
            if (responseString == "ILLEGAL REQUEST") {
                //Registration Was No Corret
            }
            if (responseString.Length > 10 && responseString.Substring(0, 10) == "Registered") {
                //Registration Complete
            }
            if (responseString == "ERROR") {
                //Some sort of error
            }
            formNick = ""; //just clean our variables
            formPassword = "";

        }
    }
}
