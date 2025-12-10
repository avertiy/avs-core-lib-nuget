using System;

namespace AVS.CoreLib.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class PercentAttribute : Attribute
{
    public PercentFormat Format { get; }

    public PercentAttribute(PercentFormat format)
    {
        Format = format;
    }
}

public enum PercentFormat
{
    /// <summary>
    /// 0.5 means 50%
    /// </summary>
    Fraction,
    /// <summary>
    /// 0.5 means 0.5%
    /// </summary>
    Whole
}