using System;

namespace AVS.CoreLib.Abstractions.Responses
{
    /// <summary>
    /// allows to substitute ResponseFactory (through singleton Instance)
    /// if you want to inherit from Response to keep Response.Create(...) methods work with your inherited response
    /// </summary>
    public interface IResponseFactory
    {
        IResponse Create(string source = null, string error = null, dynamic props = null);
        IResponse<T> Create<T>(T data, string source = null, string error = null, dynamic props = null);
        IResponse<T> Create<T>(string source = null, string error = null, dynamic props = null);
        IResponse<T> Create<T>(T data, Exception ex);
    }
}