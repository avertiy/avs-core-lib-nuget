namespace AVS.CoreLib.BootstrapTools.Schedule;

internal class ScheduledTaskEntry
{
    public string Name { get; set; } = null!;
    internal DateTime? LastRun { get; set; }
    public IScheduledTask Task { get; set; } = null!;
    public Exception? Error { get; set; }
    public bool IgnoreError { get; set; } = false;
    public DateTime? StartTime { get; set; }
    public int? StartDelay { get; set; }
    public DateTime? EndTime { get; set; }
    public int Interval { get; set; }
    internal Scheduler Scheduler { get; set; } = null!;

    public bool ShouldRun(DateTime utcTime)
    {
        if (Interval <= 0)
            return false;

        if (Task.Paused)
            return false;

        if (StartTime.HasValue && StartTime > utcTime || EndTime.HasValue && EndTime <= utcTime)
            return false;

        if (Error != null && !IgnoreError)
            return false;

        if (StartDelay.HasValue && (utcTime - Scheduler.Started).TotalSeconds < StartDelay)
            return false;

        if (!LastRun.HasValue)
            return true;

        var ts = utcTime - LastRun.Value;
        return ts.TotalSeconds > Interval;
    }
}