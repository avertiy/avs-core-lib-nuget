using System;
using System.Diagnostics;
using System.Text;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.Debugging
{
    public static class XDebug
    {
        public static string Dump<T>(this T obj, bool printToConsole = false)
        {
            var sb = new StringBuilder();
            var type = obj.GetType();

            //sb.AppendLine($"Dump of {type.GetReadableName()}:");

            var dump = ObjectDumper.Dump(obj);
            sb.AppendLine(dump);

            var text = sb.ToString();
            if (printToConsole)
                Console.WriteLine(text);

            Debug.WriteLine(text);
            return text;
        }

        [DebuggerStepThrough]
        public static void RunSync(Action action)
        {
            SyncUtil.RunSync(action);
        }
    }
}