using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AVS.CoreLib.Debugging
{
    public static class BinarySerializer
    {
        public static TData Deserialize<TData>(string text)
        {
            var b = Convert.FromBase64String(text);
            using (var stream = new MemoryStream(b))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return (TData)formatter.Deserialize(stream);
            }
        }

        public static string Serialize<TData>(TData obj)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Flush();
                stream.Position = 0;
                return Convert.ToBase64String(stream.ToArray());
            }
        }
    }
}