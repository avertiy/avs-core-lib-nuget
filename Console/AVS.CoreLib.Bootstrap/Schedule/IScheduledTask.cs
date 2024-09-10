namespace AVS.CoreLib.BootstrapTools.Schedule;

/// <summary>
/// Scheduled task is a task that is run every <see cref="Interval"/> seconds in a background thread by means of <seealso cref="IScheduler"/>
/// </summary>
public interface IScheduledTask
{
    /// <summary>
    /// interval in seconds
    /// </summary>
    int Interval { get; set; }
    int? StartDelay { get; set; }
    DateTime? StartTime { get; set; }
    DateTime? EndTime { get; set; }
    bool Paused { get; set; }
    void Invoke();
}

public class ScheduleOptions
{
    public string? Name { get; set; }
    /// <summary>
    /// Interval in seconds
    /// </summary>
    public int Interval { get; set; }
    public bool IgnoreError { get; set; }
    public int? StartDelay { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public static ScheduleOptions Default { get; } = new();
}

public abstract class ScheduledTask : IScheduledTask
{
    public int Interval { get; set; }
    public int? StartDelay { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool Paused { get; set; }
    public virtual void Invoke()
    {
        InvokeAsync().Wait();
    }

    public virtual Task InvokeAsync()
    {
        return Task.CompletedTask;
    }
}