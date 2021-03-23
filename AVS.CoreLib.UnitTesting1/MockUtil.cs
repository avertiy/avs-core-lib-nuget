using Microsoft.Extensions.Options;
using Moq;

namespace AVS.CoreLib.UnitTesting
{
    public static class MockUtil
    {
        public static IOptionsSnapshot<TOptions> MockOptions<TOptions>(TOptions options, string name) where TOptions : class, new()
        {
            var mock = new Mock<IOptionsSnapshot<TOptions>>();
            mock.Setup(m => m.Get(name)).Returns(options);
            return mock.Object;
        }
    }
}
