namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IPageable
    {
        void ApplyPageOptions(IPageOptions options);
    }
}