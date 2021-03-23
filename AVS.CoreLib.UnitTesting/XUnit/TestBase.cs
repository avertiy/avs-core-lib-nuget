
using Xunit.Abstractions;

namespace AVS.CoreLib.UnitTesting.xUnit
{
    public abstract class TestBase
    {
        private readonly ITestOutputHelper _output;

        protected TestBase(ITestOutputHelper output)
        {
            _output = output;
        }

        protected void WriteLine(string format, params string[] args)
        {
            _output.WriteLine(format, args);
        }

        protected void WriteLine(string message)
        {
            _output.WriteLine(message);
        }
    }
}