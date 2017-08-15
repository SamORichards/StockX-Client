using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StockMarketDesktopClient.Scripts {
    class Helper {
        public static TextBlock CreateTextBlock(string Text, TextAlignment al, int Width, int fontSize) {
            TextBlock block = new TextBlock();
            block.Text = Text;
            block.TextAlignment = al;
            if (Width != 0) {
                block.Width = Width;
            }
            block.FontSize = fontSize;
            return block;
        }
    }
}
