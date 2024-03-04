using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AVS.CoreLib.Debugging
{
    [DebuggerStepThrough]
    public class TryContext<T, TResult>
    {
        private static int Counter = 1000;
        public T Obj { get; }
        public TResult? Result { get; set; }
        public Exception? Error { get; private set; }
        public Exception? InnerException => Error?.InnerException;
        public string? Label { get; set; }

        public Stopwatch Timer = new Stopwatch();

        private Action<T>? Action { get; }
        private Func<T, TResult>? Func { get; }
        private Func<T, Task<TResult>>? FuncAsync { get; }

        private Action<Exception>? _onError;
        private Action<TResult>? _onSuccess;
        private Func<T, bool>? _breakpointCondition;
        private Func<T, T>? _breakpointCallback;
        private bool _success = false;

        [DebuggerStepThrough]
        public TryContext(T obj, Action<T> action, string? label = null)
        {
            Obj = obj;
            Action = action;
            Label = label;
            Counter++;
        }

        [DebuggerStepThrough]
        public TryContext(T obj, Func<T, TResult> func, string? label = null)
        {
            Obj = obj;
            Func = func;
            Label = label;
            Counter++;
        }

        [DebuggerStepThrough]
        public TryContext(T obj, Func<T, Task<TResult>> func, string? label = null)
        {
            Obj = obj;
            FuncAsync = func;
            Label = label;
            Counter++;
        }

        [DebuggerStepThrough]
        public TryContext<T, TResult> Breakpoint(Func<T, bool> condition, Func<T, T> callback)
        {
            _breakpointCondition = condition;
            _breakpointCallback = callback;
            return this;
        }

        [DebuggerStepThrough]
        public TryContext<T, TResult> OnError(Action<Exception> action)
        {
            if (Error != null)
            {
                action?.Invoke(Error);
            }

            _onError = action;
            return this;
        }

        [DebuggerStepThrough]
        public TryContext<T, TResult> FakeResult(Func<T, TResult> func)
        {
            SetResult(func(Obj));
            return this;
        }

        private void SetResult(TResult result)
        {
            if (Result == null || Result.Equals(default))
            {
                Result = result;
            }
        }

        [DebuggerStepThrough]
        public TryContext<T, TResult> OnSuccess(Action<TResult> action)
        {
            if (Error == null && _success && Result != null)
            {
                action?.Invoke(Result);
            }

            _onSuccess = action;
            return this;
        }

        [DebuggerStepThrough]
        public TryContext<T, TResult> OnSuccess(Func<TResult, bool> condition, Action<TResult> callback)
        {
            if (Error == null && _success && Result != null)
            {
                if (condition?.Invoke(Result) ?? false)
                {
                    callback?.Invoke(Result);
                }
            }

            return this;
        }


        [DebuggerStepThrough]
        public TryContext<T, TResult> OnError(Action<Exception, TryContext<T, TResult>> action)
        {
            _onError = x => action(x, this);
            return this;
        }

        [DebuggerStepThrough]
        public TryContext<T, TResult> Catch(out Exception? error)
        {
            Execute();
            error = Error;
            return this;
        }

        [DebuggerStepThrough]
        public TryContext<T, TResult> Catch()
        {
            Execute();
            return this;
        }

        [DebuggerStepThrough]
        public TryContext<T, TResult> Execute(out string? error)
        {
            var res = Execute();
            error = Error?.ToString();
            return this;
        }

        [DebuggerStepThrough]
        public TryContext<T, TResult> Execute()
        {
            var label = Label ?? Counter.ToString();
            try
            {
                Error = null;
                Debug.WriteLine($"Executing TRY {label}:");
                AttachDebugger();
                Timer.Start();

                if (Func != null)
                {
                    SetResult(Func(Obj));
                    Timer.Stop();
                    Debug.WriteLine($"TRY {label} => SUCCESS ({Timer.ElapsedMilliseconds} ms)");
                    Debug.WriteLine($"RESULT: {Result}");
                    _success = true;
                    _onSuccess?.Invoke(Result!);
                    return this;
                }

                Action?.Invoke(Obj);
                Timer.Stop();
                Debug.WriteLine($"TRY {label} => SUCCESS ({Timer.ElapsedMilliseconds} ms)");
                _success = true;
                _onSuccess?.Invoke(Result!);
            }
            catch (Exception ex)
            {
                Timer.Stop();
                _onError?.Invoke(ex);
                Error = ex;
                Debug.WriteLine($"TRY {label} => ERROR:\r\n{ex}");
            }
            return this;
        }

        [DebuggerStepThrough]
        public async Task<TryContext<T, TResult>> ExecuteAsync()
        {
            var label = Label ?? Counter.ToString();
            try
            {
                Error = null;
                Debug.WriteLine($"Executing TRY {label}:");
                AttachDebugger();
                Timer.Start();

                if (FuncAsync !=null)
                {
                    var res = await FuncAsync(Obj);
                    SetResult(res);
                    Timer.Stop();
                    Debug.WriteLine($"TRY {label} => SUCCESS ({Timer.ElapsedMilliseconds} ms)");
                    Debug.WriteLine($"RESULT: {Result}");
                    _success = true;
                    _onSuccess?.Invoke(Result!);
                    return this;
                }

                //Action?.Invoke(Obj);
                //Timer.Stop();
                //Debug.WriteLine($"TRY {label} => SUCCESS ({Timer.ElapsedMilliseconds} ms)");
                //_success = true;
                //_onSuccess?.Invoke(Result);
            }
            catch (Exception ex)
            {
                Timer.Stop();
                _onError?.Invoke(ex);
                Error = ex;
                Debug.WriteLine($"TRY {label} => ERROR:\r\n{ex}");
            }
            return this;
        }

        public static implicit operator TResult?(TryContext<T, TResult> ctx)
        {
            return ctx.Result;
        }

        private void AttachDebugger()
        {
            if (_breakpointCondition != null && _breakpointCallback != null)
            {
                var attachDebugger = _breakpointCondition.Invoke(Obj);
                if (attachDebugger)
                    _breakpointCallback(Obj);
            }
        }
    }
}