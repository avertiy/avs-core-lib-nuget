using System.Diagnostics;

namespace AVS.CoreLib.REST.Responses
{
    [DebuggerDisplay("Result = {Result}, Error = {Error}")]
    public class BoolResponse : Response
    {
        public bool Result { get; set; }

        public static implicit operator Response<bool>(BoolResponse r)
        {
            return new Response<bool>() { Data = r.Result, Error = r.Error, Source = r.Source };                
        }
    }
}