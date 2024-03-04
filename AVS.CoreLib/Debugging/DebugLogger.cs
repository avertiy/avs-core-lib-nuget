using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Debugging
{
    public class DebugLogger
    {
        private List<string> _log = new List<string>();
        private int _counter = 0;
        //this is just to lookup with what args a method has been called;
        private Dictionary<int, object?> _stack = new();

        public Dictionary<int, object?> CallStack => _stack;
        public List<string> Logs => _log;

        public void Log(string method, params object[] args)
        {
            _counter++;
            _stack.Add(_counter, args);

            if (args.Length == 0)
            {
                _log.Add($"#{_counter} {method}()");
                _stack.Add(_counter, null);
            }
            else if (args.Length == 1)
            {
                _log.Add($"#{_counter} {method}({args[0]})");
                _stack.Add(_counter, args[0]);
            }
            else
            {
                var argsStr = string.Join(", ", args.Select(x => x?.ToString() ?? "null"));
                _log.Add($"#{_counter} {method}({argsStr})");
                _stack.Add(_counter, args.Length);
            }
            Shrink();
        }

        public void Shrink(int n = 100)
        {
            // shrink old records
            if (_stack.Count > n)
            {
                var dict = _stack.Skip(n).ToDictionary(x => x.Key - 100, x => x.Value);
                _stack = dict;

            }

            if (_log.Count > n)
                _log = _log.Skip(n).ToList();
        }
    }
}
