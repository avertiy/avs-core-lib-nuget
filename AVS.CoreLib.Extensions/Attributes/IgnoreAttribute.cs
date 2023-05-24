using System;

namespace AVS.CoreLib.Extensions.Attributes
{
    /// <summary>
    /// generic purpose attribute mark property/field/whatever to be ignored e.g. when print object
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class IgnoreAttribute : Attribute
    {
    }

    /// <summary>
    /// generic purpose attribute mark property/field/whatever as important so it can't be escaped from printing even when it is empty (by default we don't print empty props)
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ImportantAttribute : Attribute
    {
    }
}