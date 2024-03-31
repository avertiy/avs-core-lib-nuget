using System;

namespace AVS.CoreLib.Text.FormatPreprocessors
{
    /// <summary>
    /// Argument format preprocessor
    /// it allows to modify format of arguments of <see cref="FormattableString"/> just before converting to string
    /// i.e. <see cref="string.Format(System.IFormatProvider,string,object)"/> is called
    /// 
    /// the purpose of the modifier is to add some behavior for adding color formatting to string arguments if color was not specified   
    /// </summary>
    public interface IFormatPreprocessor
    {
        /// <summary>
        /// Modify argument string format
        /// </summary>
        /// <returns>new argument format</returns>
        string Process(string format, object argument);
    }
}