namespace AVS.CoreLib.Abstractions
{
    public interface ICloneable<out T>
    {
        T Clone();
    }
}