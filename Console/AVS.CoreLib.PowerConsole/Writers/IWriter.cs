using System;
using System.IO;
using System.Text;

namespace AVS.CoreLib.PowerConsole.Writers
{
    public interface IWriter
    {
        void Write(string str, bool endLine);
        void Write(string str, ConsoleColor color, bool endLine);
        void WriteLine(bool voidMultipleEmptyLines = true);
    }


    //public class StreamWriter : IWriter
    //{
    //    StringWriter _output;

    //    public StreamWriter()
    //    {
    //        var ssw = new StreamWriter();
    //        var sw = new StringWriter();
    //        sw.w
    //    }

    //    public StreamWriter(StringBuilder sb)
    //    {
    //        _output = output;
    //    }

    //    public void Write(string str, bool endLine)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Write(string str, ConsoleColor color, bool endLine)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void WriteLine(bool voidMultipleEmptyLines = true)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}