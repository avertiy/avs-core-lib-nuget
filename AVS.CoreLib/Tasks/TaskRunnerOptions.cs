namespace AVS.CoreLib.Extensions.Tasks;

public struct TaskRunnerOptions
{
    public int BatchSize;
    public int BatchTimespan;
    public int Delay;
    public int Timeout;
    public TaskRunnerStrategy Strategy;

    public TaskRunnerOptions(int delay = 0, int timeout = 0, int batchSize = 0, int batchTimespan = 0, TaskRunnerStrategy strategy = TaskRunnerStrategy.RunAll)
    {
        BatchSize = batchSize;
        BatchTimespan = batchTimespan;
        Delay = delay;
        Timeout = timeout;
        Strategy = strategy;
    }
}

public enum TaskRunnerStrategy
{
    RunAll = 0,
    ExecuteOneThenAll = 1
}