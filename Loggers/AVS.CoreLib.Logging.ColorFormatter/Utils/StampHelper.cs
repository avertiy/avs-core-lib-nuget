namespace AVS.CoreLib.Logging.ColorFormatter.Utils;

public static class StampHelper
{
    public static bool MatchTimeStamp(string message, DateTimeOffset time, out string result)
    {
        switch (message)
        {
            case "[time]":
                result = time.ToString("T");
                return true;
            case "[timestamp]":
                result = time.ToString("G");
                return true;
            case "[date]":
                result = time.ToString("d");
                return true;
            default:
                result = null;
                return false;
        }
    }

    public static bool MatchBreakLine(string message)
    {
        switch (message)
        {
            case "[br]":
                return true;
            default:
                return false;
        }
    }
}