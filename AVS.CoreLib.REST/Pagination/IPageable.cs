namespace AVS.CoreLib.REST.Pagination
{
    public interface IPageable
    {
        void ApplyPageOptions(PageOptions options);
    }
}