using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.BootstrapTools.Schedule;

public interface ISchedulerBuilder
{
    ISchedulerBuilder Schedule<TScheduledTask>(ScheduleOptions? options = null)
        where TScheduledTask : class, IScheduledTask;
    ISchedulerBuilder OnError(Action<string, Exception> onError);
}

public class SchedulerBuilder : ISchedulerBuilder
{
    private readonly Scheduler _scheduler = new();
    public IServiceProvider ServiceProvider { get; set; } = null!;
    public ISchedulerBuilder Schedule<TScheduledTask>(ScheduleOptions? options = null) where TScheduledTask : class, IScheduledTask
    {
        var task = ServiceProvider.GetRequiredService<TScheduledTask>();
        options ??= ScheduleOptions.Default;
        var entry = new ScheduledTaskEntry()
        {
            Name = options.Name ?? task.GetType().Name,
            IgnoreError = options.IgnoreError,
            Task = task,
            StartTime = task.StartTime ?? options.StartTime,
            EndTime = task.EndTime ?? options.EndTime,
            StartDelay = task.StartDelay ?? options.StartDelay,
            Interval = task.Interval > 0 ? task.Interval : options.Interval
        };
        _scheduler.Add(entry);
        return this;
    }

    public ISchedulerBuilder OnError(Action<string, Exception> onError)
    {
        _scheduler.ErrorHandler = (x) => onError(x.Name, x.Error!);
        return this;
    }

    public IScheduler GetScheduler()
    {
        return _scheduler;
    }
}