using System.Diagnostics;
using System.Text.Json.Serialization;
using AVS.CoreLib.Json;

namespace AVS.CoreLib.Structs;
/// <summary>
/// EV (expplained value) represents a value/reason pair, helps to keep explained calculations with multiple conditions
/// <code>
///     var orderSize = GetOrderSize(..);
///     GetOrderSize(..) { 
///         if (A) 
///             return (orderSize.L, "Beacuse of A") 
///         if (B) 
///             return (orderSize.XL, "Beacuse of B")
///         ...
///     };
/// </code>
/// </summary>
[JsonConverter(typeof(ArrayConverter))]
[DebuggerDisplay("{ToString()}")]
public readonly record struct EV<T>(T Value, string Reason)
{
    public static implicit operator T(EV<T> value)
        => value.Value;

    public static implicit operator EV<T>(T value) => new(value, string.Empty);

    public static implicit operator EV<T>((T Value, string Reason) tupple)
        => new(tupple.Value, tupple.Reason);

    public override string ToString()
        => string.IsNullOrEmpty(Reason) ? $"{Value}" : $"{Value}: {Reason}";
}
