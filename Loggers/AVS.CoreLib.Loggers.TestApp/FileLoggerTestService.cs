using AVS.CoreLib.Abstractions;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Loggers.TestApp
{
    public class FileLoggerTestService : ITestService
    {
        private readonly ILogger _logger;

        public FileLoggerTestService(ILogger<FileLoggerTestService> logger)
        {
            _logger = logger;
        }

        public void Demo()
        {
            
        }

        public void Test()
        {
            _logger.LogInformation("test info log");
            _logger.LogWarning("test warning log");
            _logger.LogError("test error log");
        }
    }
}