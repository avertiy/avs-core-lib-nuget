#nullable enable
using System;
using System.Collections.Generic;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Guards;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace AVS.CoreLib.REST.Models
{
    public class ParametersCollection : Dictionary<string, object>, IParametersCollection
    {
    }

    
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member