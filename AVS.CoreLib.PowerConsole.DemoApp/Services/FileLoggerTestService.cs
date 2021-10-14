using AVS.CoreLib.Abstractions;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.PowerConsole.DemoApp.Services
{
    public class FileLoggerTestService : ITestService
    {
        private readonly ILogger _logger;

        public FileLoggerTestService(ILogger<FileLoggerTestService> logger)
        {
            _logger = logger;
        }

        public void Test()
        {
            _logger.LogDebug("debug log");
            _logger.LogInformation("info log");
            _logger.LogWarning("warning log");
            _logger.LogError("error log");
        }
    }
}