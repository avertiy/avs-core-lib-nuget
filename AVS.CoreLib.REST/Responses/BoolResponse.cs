using System.Diagnostics;

namespace AVS.CoreLib.REST.Responses
{
    [DebuggerDisplay("Result = {Result}, Error = {Error}")]
    public class BoolResponse : Response
    {
        public bool Result { get; set; }

        public static implicit operator Response<bool>(BoolResponse r)
        {
            return Response.Create<bool>(r.Result, r.Source, r.Error, r.Request);
        }
    }
}