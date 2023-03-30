using System;

namespace AVS.CoreLib.REST.Projections
{
    public class MapException : AppException
    {
        public string JsonText { get; set; }
        public MapException(string message) : base(message)
        {
        }

        public MapException(string message, string hint) : base(message, hint)
        {
        }

        public MapException(string message, Exception error) : base(message, error)
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