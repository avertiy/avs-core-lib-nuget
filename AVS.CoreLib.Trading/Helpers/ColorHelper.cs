using System;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Helpers
{
    public static class ColorHelper
    {
        public static ConsoleColor GetColor(this OrderSide side)
        {
            return side == OrderSide.Buy ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed;
        }

        public static ConsoleColor GetColor(this TradeType type)
        {
            return type == TradeType.Buy ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed;
        }
    }
}