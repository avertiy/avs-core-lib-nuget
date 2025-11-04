using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Diagnostic
{
    public class DebugHelper
    {
        public List<string> Logs { get; private set; } = new(200);

        public int Count => Logs.Count;
        //this is to track args the method has been invoked with;
        public Dictionary<int, object?> Stack { get; private set; } = new(200);

        public void Log(string method, params object[] args)
        {
            //_stack.Add(_counter, args);
            var counter = Logs.Count + 1;
            if (args.Length == 0)
            {
                Logs.Add($"#{counter} {method}()");
                Stack.Add(counter, null);
            }
            else if (args.Length == 1)
            {
                Logs.Add($"#{counter} {method}({args[0]})");
                Stack.Add(counter, args[0]);
            }
            else
            {
                var argsStr = string.Join(", ", args.Select(x => x?.ToString() ?? "null"));
                Logs.Add($"#{counter} {method}({argsStr})");
                Stack.Add(counter, args.Length);
            }
            Shrink();
        }

        

        public void Shrink(int n = 100)
        {
            // shrink old records
            if (Stack.Count > n)
            {
                var dict = Stack.Skip(n).ToDictionary(x => x.Key - 100, x => x.Value);
                Stack = dict;
            }

            if (Logs.Count > n)
                Logs = Logs.Skip(n).ToList();
        }
    }
}
