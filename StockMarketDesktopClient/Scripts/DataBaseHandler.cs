using System;
using Pomelo.Data.MySql;
using System.Data;

namespace StockMarketDesktopClient.Scripts {
    class DataBaseHandler {
        public static string Nickname;
        public static int UserID;
        static string myConnectionString = "server=sammyben.ddns.net;database=stockmarket;uid=Sam;pwd=230999;";
        public static MySqlConnection sqlCon = new MySqlConnection(myConnectionString);
        public static void StartServer() {
            OpenConnection();
        }

        public static MySqlDataReader GetData(string command) {
            ReadyConnection();
            MySqlCommand com = new MySqlCommand(command, sqlCon);
            MySqlDataReader reader;
            reader = com.ExecuteReader();
            return reader;
        }


        public static void SetData(string command) {
            ReadyConnection();
            MySqlCommand com = new MySqlCommand(command, sqlCon);
            com.ExecuteNonQuery();
        }

        public static int GetCount(string command) {
            ReadyConnection();
            MySqlCommand com = new MySqlCommand(command, sqlCon);
            string t = com.ExecuteScalar().ToString();
            if (t.Length == 0) {
                return 0;
            } else {
                return int.Parse(t);
            }
        }

        public static double GetCountDouble(string command) {
            ReadyConnection();
            MySqlCommand com = new MySqlCommand(command, sqlCon);
            string t = com.ExecuteScalar().ToString();
            if (t.Length == 0) {
                return 0;
            } else {
                return double.Parse(t);
            }
        }




        static void ReadyConnection() {
            try {
                sqlCon.Close();
            } catch { }
            StartServer();
        }


        private static void OpenConnection() {
            for (int i = 0; i < 10; i++) {
                try {
                    sqlCon.Open();
                    break;
                } catch {
                    if (i == 9) {
                        throw new Exception("Failed to connect to database");
                    }
                }
            }
        }
    }
}
