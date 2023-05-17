using System;

namespace AVS.CoreLib.Extensions.Attributes
{
    /// <summary>
    /// generic purpose attribute mark property/field whatever to be ignored e.g. when print object
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class IgnoreAttribute : Attribute
    {
    }
}