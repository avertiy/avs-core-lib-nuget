using System;
using System.Diagnostics;

namespace AVS.CoreLib.Structs
{
    /*
    /// <summary>
    /// <code>
    ///  //usage:
    ///  var str = "Position Size {Size}" + new ConditionalString(ReaplizedPNL > 0, $"ReaplizedPNL: {ReaplizedPNL}");    
    /// </code>
    /// </summary>
    [DebuggerDisplay("ConditionalString: {ToString()}")]
    [Obsolete("Use str.AppendIf(condition, text)")]
    public readonly struct ConditionalString
    {
        public string Text { get; }

        public ConditionalString(bool condition, string text)
        {
            Text = condition ? text : string.Empty;
        }

        public ConditionalString(bool condition, Func<string> text)
        {
            Text = condition ? text() : string.Empty;
        }

        public static implicit operator string(ConditionalString value)
            => value.Text;

        public override string ToString()
            => Text;
    }*/
}