namespace AVS.CoreLib.BootstrapTools.Schedule;

public interface IScheduler
{
    void Start();
    void Stop();
}

internal class Scheduler : IScheduler
{
    private readonly List<ScheduledTaskEntry> _entries = new();
    public Action<ScheduledTaskEntry>? ErrorHandler { get; set; }
    private bool _stop;

    public int Count => _entries.Count;

    public int ShortestInterval => _entries.Min(x => x.Interval);

    public DateTime Started { get; private set; }

    public void Add(ScheduledTaskEntry entry)
    {
        entry.Scheduler = this;
        _entries.Add(entry);
    }

    public void Start()
    {
        if (!_entries.Any())
            return;

        _stop = false;
        Task.Run(async () =>
        {
            Started = DateTime.UtcNow;
            while (!_stop && _entries.Any())
            {
                var utcNow = DateTime.UtcNow;
                await this.RunAtAsync(utcNow);
                var elapsed = DateTime.UtcNow - utcNow;

                var shortestInterval = ShortestInterval;
                var timeToSleep = (int)(shortestInterval - elapsed.TotalSeconds);

                if (timeToSleep > 0)
                {
                    Thread.Sleep(timeToSleep);
                }
            }
        });
    }

    public void Stop()
    {
        _stop = true;
    }

    private async Task RunAtAsync(DateTime utcNow)
    {
        var tasks = new List<Task>();

        foreach (var entry in _entries.Where(x => x.ShouldRun(utcNow)))
        {
            tasks.Add(Invoke(entry, utcNow));
        }

        await Task.WhenAll(tasks);
    }

    private async Task Invoke(ScheduledTaskEntry entry, DateTime time)
    {
        try
        {
            await Task.Run(entry.Task.Invoke);
        }
        catch (Exception ex)
        {
            if (!entry.IgnoreError)
            {
                entry.Error = ex;
                ErrorHandler?.Invoke(entry);
            }
        }

        entry.LastRun = time;
    }

}