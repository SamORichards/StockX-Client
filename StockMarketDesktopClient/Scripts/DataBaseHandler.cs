using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Data.Sqlite.Internal;

namespace StockMarketDesktopClient.Scripts {
    class DataBaseHandler {
        static string myConnectionString = "server=sammyben.ddns.net;database=StockMarket;uid=Sam;pwd=230999;";
        public static SqliteConnection sqlCon = new SqliteConnection(myConnectionString);
        public static void StartServer() {
            OpenConnection();
        }

        public static SqliteDataReader GetData(string command) {
            ReadyConnection();
            SqliteCommand com = new SqliteCommand(command, sqlCon);
            SqliteDataReader reader;
            //for (int i = 0; i < 10; i++) {
            //try {
            reader = com.ExecuteReader();
            return reader;
            //} 
            //catch {
            //    if (i == 9) {
            //        try {
            //            sqlCon.Close();
            //        } catch { }
            //        OpenConnection();
            //        i = 0;
            //    }
            //}
            //}
        }


        public static void SetData(string command) {
            ReadyConnection();
            SqliteCommand com = new SqliteCommand(command, sqlCon);
            //for (int i = 0; i < 10; i++) {
            //try {
            com.ExecuteNonQuery();
            //    } catch {
            //        if (i == 9) {
            //            try {
            //                sqlCon.Close();
            //            } catch { }
            //            OpenConnection();
            //            i = 0;
            //        }
            //    }
            //}
        }

        public static int GetCount(string command) {
            ReadyConnection();
            SqliteCommand com = new SqliteCommand(command, sqlCon);
            //for (int i = 0; i < 10; i++) {
            //try {
            string t = com.ExecuteScalar().ToString();
            if (t.Length == 0) {
                return 0;
            } else {
                return int.Parse(t);
            }
            //    } catch {
            //        if (i == 9) {
            //            try {
            //                sqlCon.Close();
            //            } catch { }
            //            OpenConnection();
            //            i = 0;
            //        }
            //    }
            //}
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
