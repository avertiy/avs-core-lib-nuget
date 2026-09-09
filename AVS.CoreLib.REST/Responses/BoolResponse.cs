using System;
using System.Diagnostics;

namespace AVS.CoreLib.REST.Responses
{
    [DebuggerDisplay("BoolResponse (Result = {Result}, Error = {Error})")]
    public class BoolResponse : ResponseBase
    {
        public bool Result { get; set; }

        public static implicit operator Response<bool>(BoolResponse r)
        {
            return new Response<bool>()
            {

                Source = r.Source,
                RawContent = r.RawContent,
                Error = r.Error,
                Request = r.Request,
                Data = r.Result
            };
        }
    }
}