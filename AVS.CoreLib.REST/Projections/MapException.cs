using System;
using AVS.CoreLib.Extensions;

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

        public MapException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public override string ToString()
        {
            return $"{base.ToString()}{Environment.NewLine}{Environment.NewLine}JsonText:{Environment.NewLine}{JsonText}";
        }
    }

    public class MapJsonException<T> : MapException
    {
        public MapJsonException(Exception innerException)
            : base($"Map<{typeof(T).ToStringNotation()}> failed", innerException)
        {
        }
    }

    public class MapJsonException<T1, T2> : Exception
    {
        public MapJsonException(Exception innerException)
            : base($"Map<{typeof(T1).ToStringNotation()},{typeof(T2).ToStringNotation()}> failed", innerException)
        {
        }
    }
}