using System;
using System.Diagnostics;
using System.Text;

namespace AVS.CoreLib.Debugging
{
    public static class XDebug
    {
        public static string Dump<T>(this T obj, bool printToConsole = false)
        {

            var type = obj.GetType();
            var sb = new StringBuilder();
            sb.AppendLine(type.FullName);

            var dump = ObjectDumper.Dump(obj);
            sb.AppendLine(dump);

            //string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            //sb.AppendLine(json);

            var text = sb.ToString();
            if (printToConsole)
                Console.WriteLine(text);

            Debug.WriteLine(text);
            return text;
        }
    }
}