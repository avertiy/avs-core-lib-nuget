using System;

namespace AVS.CoreLib.Mapper
{
    public class MappingNotFoundException : Exception
    {
        public MappingNotFoundException(string key) : base($"Mapping `{key}` has not been registered") { }
    }
}
