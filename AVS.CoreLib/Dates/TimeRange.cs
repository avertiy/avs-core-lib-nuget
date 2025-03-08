using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Dates;

public readonly struct TimeRange
{
    public long StartTime { get; }
    public long EndTime { get; }

    #region Auxilary (auto-calc) props
    public bool IsInMilliseconds => DateTimeHelper.IsInMilliseconds(StartTime);
    public DateTime StartDateTime => DateTimeHelper.FromUnixTimestamp(StartTime);
    public DateTime EndDateTime => DateTimeHelper.FromUnixTimestamp(EndTime);
    public double TotalDays => (EndDateTime - StartDateTime).TotalDays;
    public double TotalSeconds => (EndDateTime - StartDateTime).TotalSeconds;
    public double TotalMilliseconds => (EndDateTime - StartDateTime).TotalMilliseconds;
    public int Days => Convert.ToInt32((EndTime - StartTime) / (DateTimeHelper.SECONDS_IN_DAY * 1000));
    public long Seconds => (EndTime - StartTime) / 1000;
    public long Milliseconds => EndTime - StartTime; 
    #endregion

    public TimeRange(long startTime, long endTime)
    {
        if (startTime > endTime)
            throw new ArgumentOutOfRangeException($"StartTime {startTime} must be less than EndTime {endTime} (diff={startTime - endTime})");

        StartTime = startTime;
        EndTime = endTime;
    }

    public bool Contains(long time)
    {
        return StartTime <= time && EndTime >= time;
    }

    public bool Contains(TimeRange range)
    {
        return StartTime <= range.StartTime && EndTime >= range.EndTime;
    }

    public override string ToString()
    {
        return $"[{StartTime};{EndTime}]";
    }

    #region Compare operators overloading
    public bool Equals(TimeRange other)
    {
        return StartTime.Equals(other.StartTime) && EndTime.Equals(other.EndTime);
    }

    public override bool Equals(object? obj)
    {
        return obj is DateRange other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StartTime, EndTime);
    }

    public static bool operator ==(TimeRange dateRange, TimeRange compare)
    {
        return Math.Abs(dateRange.TotalMilliseconds - compare.TotalMilliseconds) < 1;
    }

    public static bool operator !=(TimeRange dateRange, TimeRange compare)
    {
        return Math.Abs(dateRange.TotalMilliseconds - compare.TotalMilliseconds) > 0.1;
    }

    public static bool operator >=(TimeRange dateRange, TimeRange compare)
    {
        return dateRange.TotalMilliseconds >= compare.TotalMilliseconds;
    }

    public static bool operator <=(TimeRange dateRange, TimeRange compare)
    {
        return dateRange.TotalMilliseconds <= compare.TotalMilliseconds;
    }
    public static bool operator >(TimeRange dateRange, TimeRange compare)
    {
        return dateRange.TotalMilliseconds > compare.TotalMilliseconds;
    }

    public static bool operator <(TimeRange dateRange, TimeRange compare)
    {
        return dateRange.TotalMilliseconds < compare.TotalMilliseconds;
    }

    #endregion


    public static implicit operator DateRange(TimeRange timeRange)
    {
        return new DateRange(timeRange.StartDateTime, timeRange.EndDateTime);
    }

    public static implicit operator TimeRange(DateRange dateRange)
    {
        return new TimeRange(dateRange.From.ToUnixTimeMs(), dateRange.To.ToUnixTimeMs());
    }

    public static bool TryParseExact(string str, out TimeRange range)
    {
        range = default;

        var parts = str.Split(';', StringSplitOptions.RemoveEmptyEntries);
            
        if (parts.Length != 2)
            return false;

        if (long.TryParse(parts[0].TrimStart('['), out var startTime) && long.TryParse(parts[1].TrimStart(']'), out var endTime))
        {
            range = new TimeRange(startTime, endTime);
            return true;
        }

        return false;
    }
}


public static class TimeRangeExtensions
{
    public static IEnumerable<TimeRange> Slice(this TimeRange range, int milliseconds)
    {
        var interval = range.IsInMilliseconds ? milliseconds : milliseconds / 1000;

        for (var i = range.StartTime; i < range.EndTime;)
        {
            var next = i + interval;

            if (next > range.EndTime)
                next = range.EndTime;

            yield return new TimeRange(i, next);
            i = next;
        }
    }

    /// <summary>
    /// Take the earlier StartTime and the latest EndTime
    /// </summary>
    public static TimeRange Extend(this TimeRange range, TimeRange other)
    {
        var startTime = range.StartTime <= other.StartTime ? range.StartTime : other.StartTime;
        var endTime = range.EndTime >= other.EndTime ? range.EndTime : other.EndTime;
        return new TimeRange(startTime, endTime);
    }

    /// <summary>
    /// Take latest StartTime and the earliest EndTime
    /// </summary>
    public static TimeRange Shrink(this TimeRange range, TimeRange other)
    {
        var startTime = range.StartTime >= other.StartTime ? range.StartTime : other.StartTime;
        var endTime = range.EndTime <= other.EndTime ? range.EndTime : other.EndTime;
        return new TimeRange(startTime, endTime);
    }
}