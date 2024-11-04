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
            return Response.Create<bool>(r.Result,r.RawContent, r.Source, r.Error, r.Request);
        }
    }
}