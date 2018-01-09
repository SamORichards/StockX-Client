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
using Windows.UI;
using Windows.UI.Popups;
using System.Text;
using System.Security.Cryptography;

namespace StockMarketDesktopClient {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page {
        public MainPage() {
            this.InitializeComponent();
        }

        private async void LoginButtonClick(object sender, RoutedEventArgs e) {
            DataBaseHandler.UserID = 1;
            if (!(ValidEmail(EmailBox.Text))) {
                MessageDialog message = new MessageDialog("Email is not valid");
                await message.ShowAsync();
                return;
            }
            if (!(ValidUsernamePassword(PasswordBox.Password))) {
                MessageDialog message = new MessageDialog("Password is not valid");
                await message.ShowAsync();
                return;
            }
            OnlineConnector("Login", "", PasswordBox.Password, EmailBox.Text);
        }

        private async void CompleteRegistrationClick(object sender, RoutedEventArgs e) {
            if (!(ValidEmail(EmailBox.Text))) {
                MessageDialog message = new MessageDialog("Email is not valid");
                await message.ShowAsync();
                return;
            }
            if (!(ValidUsernamePassword(PasswordBox.Password))) {
                MessageDialog message = new MessageDialog("Password is not valid");
                await message.ShowAsync();
                return;
            }
            if (!(ValidUsernamePassword((StackList.Children[0] as TextBox).Text))) {
                MessageDialog message = new MessageDialog("Username is not valid");
                await message.ShowAsync();
                return;
            }
            OnlineConnector("Registration", (StackList.Children[0] as TextBox).Text, PasswordBox.Password, EmailBox.Text);
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

        public string CalculateMD5Hash(string input) {

            // step 1, calculate MD5 hash from input

            MD5 md5 = MD5.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(input);

            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++) {

                sb.Append(hash[i].ToString());

            }

            return sb.ToString();
        }


        async void OnlineConnector(string Act, string formNick, string formPassword, string email) {
            formPassword = CalculateMD5Hash(formPassword);
            if (Act == "Login") {
                MySqlDataReader reader = DataBaseHandler.GetData("SELECT * FROM Users WHERE Email = '" + email + "'");
                string Nickname = "";
                int UserId = -1;
                string Password = "";
                while (reader.Read()) {
                    Nickname = (string)reader["Nickname"];
                    UserId = (int)reader["ID"];
                    Password = (string)reader["Password"];
                }
                if (UserId >= 0) {
                    if (Password == formPassword) {
                        DataBaseHandler.Nickname = Nickname;
                        DataBaseHandler.UserID = UserId;
                        this.Frame.Navigate(typeof(Pages.FeaturedStock));
                    } else {
                        MessageDialog Message = new MessageDialog("The Password is incorrect");
                        await Message.ShowAsync();
                        return;
                    }
                }
            } else {
                int taken = DataBaseHandler.GetCount("SELECT COUNT(ID) FROM Users WHERE Email = '" + email + "'");
                if (taken == 0) {
                    DataBaseHandler.UserID = DataBaseHandler.GetCount(string.Format("INSERT INTO Users(Nickname, Email, Password) VALUES('{0}', '{1}', '{2}'); SELECT LAST_INSERT_ID();", formNick, email, formPassword));
                    this.Frame.Navigate(typeof(Pages.FeaturedStock));
                } else {
                    MessageDialog Message = new MessageDialog("The Email is already taken");
                    await Message.ShowAsync();
                    return;
                }
            }
        }

        private void RegisteredButtonClick(object sender, RoutedEventArgs e) {
            Buttons.Children.Remove(LoginButton);
            TextBox box = new TextBox();
            box.PlaceholderText = "Nickname";
            box.FontSize = 30;
            TextBlock text = new TextBlock();
            text.Text = "Nickname";
            text.Foreground = new SolidColorBrush(Colors.DeepSkyBlue);
            text.FontSize = 45;
            box.Header = text;
            box.Name = "NicknameField";
            RegistrationButton.Click -= RegisteredButtonClick;
            RegistrationButton.Click += CompleteRegistrationClick;
            StackList.Children.Insert(0, box);
        }
    }
}
