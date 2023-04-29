namespace AVS.CoreLib.REST.Responses
{
    public class CodeResponse : Response
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public virtual bool ShouldSerializeMessage()
        {
            return Message != null;
        }

        public virtual bool ShouldSerializeCode()
        {
            return Code > 0;
        }
    }
}