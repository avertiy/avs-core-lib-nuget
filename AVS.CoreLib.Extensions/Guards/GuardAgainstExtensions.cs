using System;

namespace AVS.CoreLib.Guards;

public static class GuardAgainstExtensions
{
    public static void NullOrEmpty(this IAgainstGuardClause guardClause, string? param, string name = "argument")
    {
        if (string.IsNullOrEmpty(param))
            throw new ArgumentNullException($"{name} must be not null or empty");
    }

    public static void NullOrEmpty(this IAgainstGuardClause guardClause, string? param, bool allowNull, string name = "argument")
    {
        if (string.IsNullOrEmpty(param) && !allowNull)
            throw new ArgumentNullException($"{name} must be not null or empty");
    }

    public static void Null(this IAgainstGuardClause guardClause, object? arg, string? message = null)
    {
        if (arg == null)
            throw new ArgumentNullException(message ?? $"must be not null");
    }

    public static void Null(this IAgainstGuardClause guardClause, object? param, bool allowNull, string name = "argument")
    {
        if (param == null && !allowNull)
            throw new ArgumentNullException($"{name} must be not null");
    }

    public static void DateTimeMin(this IAgainstGuardClause guardClause, DateTime param, string name = "argument")
    {
        if (param == DateTime.MinValue)
            throw new ArgumentNullException($"{name} must be not a DateTime.MinValue");
    }
}