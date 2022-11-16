namespace AVS.CoreLib.PowerConsole.ConsoleWriters
{
    public class ConsoleWriterBase
    {
        /// <summary>
        /// Indicates whether new line (\r\n) has been just written 
        /// </summary>
        public bool NewLineFlag = true;

        public void Write(string str, bool endLine)
        {
            System.Console.Write(str);
            if (endLine && !NewLineFlag)
            {
                System.Console.WriteLine();
                NewLineFlag = true;
            }
            else
                NewLineFlag = false;

            //NewLineFlag = str.EndsWith("\r\n");
            //if (endLine && NewLineFlag == false)
            //{
            //    System.Console.WriteLine();
            //    NewLineFlag = true;
            //}
        }

        public void WriteLine(bool voidMultipleEmptyLines = true)
        {
            if (voidMultipleEmptyLines && NewLineFlag)
                return;

            System.Console.WriteLine();
            NewLineFlag = true;
        }
    }
}