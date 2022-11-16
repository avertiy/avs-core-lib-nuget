namespace AVS.CoreLib.Abstractions.Responses
{
    public interface IResponse
    {
        string Error { get; set; }
        bool Success { get; }
        string Source { get; set; }
    }

    public interface IResponse<T> : IResponse
    {
        T Data { get; set; }
    }
}