namespace AVS.CoreLib.Guards;

/// <summary>
/// Simple interface to provide a generic mechanism to build guard clause extension methods. 
/// </summary>
/// <remarks>
/// IGuardClause idea was taken from Ardalis.GuardClauses
/// </remarks>
public interface IGuardClause
{
}

/// <summary>
/// Aggregates MustBe guard methods (Guard.MustBe.XXX)
/// </summary>
public interface IMustBeGuardClause : IGuardClause
{
}

public interface IMustBeGuardClause<T> : IGuardClause
{
    T Value { get; set; }
}

/// <summary>
/// Aggregates Must guard methods (Guard.Must.XXX)
/// </summary>
public interface IMustGuardClause : IGuardClause
{
}

/// <summary>
/// Aggregates Against guard methods (Guard.Against.XXX)
/// </summary>
public interface IAgainstGuardClause : IGuardClause
{
}

/// <summary>
/// Aggregates Items & List&lt;T&gt; guard methods (Guard.Items.XXX)
/// </summary>
public interface IArrayGuardClause : IGuardClause
{
}

/// <summary>
/// Aggregates Items & List&lt;T&gt; guard methods (Guard.Items.XXX)
/// </summary>
public interface IDictionaryGuardClause : IGuardClause
{
}