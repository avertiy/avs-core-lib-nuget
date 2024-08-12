using System;
using System.Diagnostics;

namespace AVS.CoreLib.Mapper
{
    public class MappingNotFoundException : MapException
    {
        public MappingNotFoundException(string key) : base($"Mapping `{key}` has not been registered") { }
    }

    //[DebuggerDisplay("MapException {Message} {DelegateRef}")]
    public class MapException : Exception
    {
        public string? DelegateRef { get; set; }
        public MapException(string? message) : base(message) { }
        public MapException(string? message, Exception? innerException) : base(message, innerException) { }

        public MapException(string? message, Exception? innerException, string? delegateRef)
            : base(delegateRef == null ? message : $"{message} [delegateRef:{delegateRef}]", innerException)
        {
            DelegateRef = delegateRef;
        }
    }

}
