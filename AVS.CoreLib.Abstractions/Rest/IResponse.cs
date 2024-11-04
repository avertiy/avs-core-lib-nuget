#nullable enable

namespace AVS.CoreLib.Abstractions.Responses
{
    public interface IResponse
    {
        string Source { get; set; }
        object? Request { get; set; }
        string? Error { get; set; }
        string? RawContent { get; }
        bool Success { get; }
    }

    public interface IResponse<T> : IResponse
    {
        T? Data { get; set; }
    }
}