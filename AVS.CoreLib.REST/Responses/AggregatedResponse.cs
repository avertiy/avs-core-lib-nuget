using System;
using System.Diagnostics;
using System.Linq;
using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.Collections;
using AVS.CoreLib.REST.Attributes;

namespace AVS.CoreLib.REST.Responses
{
    public interface IAggregatedResponse : IResponse
    {
        void ForEach(Action<string, dynamic> success, Action<string, string> failed = null);
    }

    /// <summary>
    /// AggregatedResponse aggregates (contains) several separate responses
    /// of the same data type
    /// </summary>
    [DebuggerDisplay("Count = {Count}, Error = {Error}")]
    public class AggregatedResponse<T> : BaseDictionary<string, Response<T>>, IAggregatedResponse
    {
        public AggregatedResponse()
        {
        }

        public AggregatedResponse(int capacity) : base(capacity)
        {
        }

        #region IResponse 
        public bool Success => Error == null;

        [ArrayProperty(-10)]
        public string Error { get; set; }

        public virtual bool ShouldSerializeError()
        {
            return Error != null;
        }

        public string Source { get; set; }

        public virtual bool ShouldSerializeSource()
        {
            return Source != null;
        }

        #endregion

        public void ForEach(Action<string, dynamic> success, Action<string, string> failed = null)
        {
            foreach (var kp in Data)
            {
                if (kp.Value.Success)
                    success(kp.Key, kp.Value.Data);
                else
                    failed?.Invoke(kp.Key, kp.Value.Error);
            }
        }

        public void ForEach(Action<string, T> success, Action<string, string> failed = null)
        {
            foreach (var kp in Data)
            {
                if (kp.Value.Success)
                    success(kp.Key, kp.Value.Data);
                else
                    failed?.Invoke(kp.Key, kp.Value.Error);
            }
        }

        public string GetTypeName()
        {
            return $"AggregatedResponse<{typeof(T).Name}>";
        }

        public override string ToString()
        {
            return Success ? $"AggregatedResponse [{string.Join(",", Keys)}]" : $"AggregatedResponse=> Fail [{Error}]";
        }

        public static implicit operator Response<T>(AggregatedResponse<T> response)
        {
            if (!response.Success)
                return new Response<T>() { Error = response.Error };

            if (response.Count == 1)
                return response.First().Value;

            var type = typeof(T).Name;
            throw new InvalidCastException($"AggregatedResponse<{type}> must contain exact one item of Response<{type}>");
        }
    }
}