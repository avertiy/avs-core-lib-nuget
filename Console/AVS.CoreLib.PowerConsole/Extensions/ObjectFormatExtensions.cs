using AVS.CoreLib.PowerConsole.ConsoleTable;

namespace AVS.CoreLib.PowerConsole.Extensions
{
    internal static class ObjectFormatExtensions
    {
        public static ColumnOptions GetColumnOptions(this ObjectFormat format)
        {
            if (format == ObjectFormat.TableHorizontal)
                return ColumnOptions.Horizontal;

            if (format == ObjectFormat.TableVertical)
                return ColumnOptions.Vertical;

            return ColumnOptions.Auto;
        }
    }
}