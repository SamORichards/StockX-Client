using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StockMarketDesktopClient.Scripts {
    class Helper {
        public static bool FirstLoad = true;

        public static TextBlock CreateTextBlock(string Text, TextAlignment al, int Width, int fontSize, int PaddingRight = 0) {
            TextBlock block = new TextBlock();
            block.Text = Text;
            block.TextAlignment = al;
            if (Width != 0) {
                block.Width = Width;
            }
            block.Padding = new Thickness(0, 0, PaddingRight, 0);
            block.FontSize = fontSize;
            return block;
        }
    }
}
