using System;
using System.Threading.Tasks;

namespace AVS.CoreLib.Extensions.Tasks;

/// <summary>
/// Little enhancement missing in Task.WhenAll - execute all tasks and return its results
/// </summary>
public static class ParallelTasks
{
    public static async Task WhenAll(this Task task, Task otherTask)
    {
        await Task.WhenAll(task, otherTask);
    }

    public static async Task<(T1, T2)> WhenAll<T1, T2>(Task<T1> task1, Task<T2> task2)
    {
        await Task.WhenAll(task1, task2);
        return (task1.Result, task2.Result);
    }

    public static async Task<(T1, T2, T3)> WhenAll<T1, T2, T3>(Task<T1> task1, Task<T2> task2, Task<T3> task3)
    {
        await Task.WhenAll(task1, task2, task3);
        return (task1.Result, task2.Result, task3.Result);
    }

    public static async Task<(T1, T2, T3, T4)> WhenAll<T1, T2, T3, T4>(Task<T1> task1, Task<T2> task2, Task<T3> task3, Task<T4> task4)
    {
        await Task.WhenAll(task1, task2, task3, task4);
        return (task1.Result, task2.Result, task3.Result, task4.Result);
    }
}