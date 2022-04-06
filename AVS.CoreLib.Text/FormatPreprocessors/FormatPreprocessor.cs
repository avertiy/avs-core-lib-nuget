using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Text.FormatPreprocessors
{
    /// <summary>
    /// Composite format preprocessor
    /// </summary>
    public class FormatPreprocessor : IFormatPreprocessor
    {
        private readonly List<IFormatPreprocessor> _preprocessors = new List<IFormatPreprocessor>();

        /// <inheritdoc />
        public string Process(string format, object argument)
        {
            if (_preprocessors.Any())
            {
                var fmt = format;
                foreach (var preprocessor in _preprocessors)
                    fmt = preprocessor.Process(fmt, argument);

                return fmt;
            }

            return format;
        }

        /// <summary>
        /// Add FormatPreprocessor
        /// </summary>
        public void Add(IFormatPreprocessor preprocessor)
        {
            _preprocessors.Add(preprocessor);
        }

        /// <summary>
        /// Clear all preprocessors
        /// </summary>
        public void Clear()
        {
            _preprocessors.Clear();
        }
    }
}