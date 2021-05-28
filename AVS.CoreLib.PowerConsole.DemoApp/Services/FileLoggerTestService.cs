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

        public void Demo()
        {
            
        }

        public void Test()
        {
            _logger.LogInformation("test info log");
        }
    }
}