using System;

namespace AVS.CoreLib.FileLogger.Common
{
    /// <summary>
    /// In ASP.NET Core 3.0 this classes is now internal. This means you need to add it to your code.
    /// </summary>
    public sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();

        private NullScope()
        {
        }

        public void Dispose()
        {
        }
    }
}