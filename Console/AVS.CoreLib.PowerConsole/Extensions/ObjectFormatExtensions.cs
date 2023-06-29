using AVS.CoreLib.PowerConsole.ConsoleTable;

namespace AVS.CoreLib.PowerConsole.Extensions
{
    internal static class ObjectFormatExtensions
    {
        public static TableOrientation GetColumnOptions(this ObjectFormat format)
        {
            if (format == ObjectFormat.TableHorizontal)
                return TableOrientation.Horizontal;

            if (format == ObjectFormat.TableVertical)
                return TableOrientation.Vertical;

            return TableOrientation.Auto;
        }
    }
}