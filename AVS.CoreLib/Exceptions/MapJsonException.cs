using System;

namespace AVS.CoreLib.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class MapJsonException : AppException
    {
        /// <summary>
        /// 
        /// </summary>
        public string? JsonText { get; set; }

        public MapJsonException(string message) : base(message)
        {
        }

        public MapJsonException(string message, string hint) : base(message, hint)
        {
        }

        public MapJsonException(string message, Exception error) : base(message, error)
        {
        }

        public override string ToString()
        {
            var s = base.ToString();
            if (string.IsNullOrEmpty(JsonText))
                return s;

            return $"{base.ToString()}{Environment.NewLine}{Environment.NewLine}JsonText: {JsonText}";
        }
    }
}