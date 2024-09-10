using System;
using AVS.CoreLib.Exceptions;
using AVS.CoreLib.Extensions.Reflection;

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

        public MapException(Exception error, ProjectionBase projection) : base($"{projection.GetTypeName()}::Map json failed.", error)
        {
            JsonText = projection.JsonText;
            Source = projection.Source;
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