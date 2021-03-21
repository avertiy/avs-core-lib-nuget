using System;

namespace AVS.CoreLib.Abstractions.Responses
{
    public interface IAggregatedResponse: IResponse
    {
        void ForEach(Action<string, dynamic> success, Action<string, string> failed = null);
    }
}