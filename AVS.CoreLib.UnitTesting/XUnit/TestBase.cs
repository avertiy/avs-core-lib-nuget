using System.Linq;
using System.Reflection;
using AVS.CoreLib.Configuration;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace AVS.CoreLib.UnitTesting.xUnit
{
    public abstract class TestBase
    {
        private readonly ITestOutputHelper _output;
        private IConfigurationRoot _configuration;

        /// <summary>
        /// use <see cref="ConfigurationAttribute"/> to set app name otherwise the AppName will be defined by entry assembly
        /// ReSharperTestRunner or whatever
        /// </summary>
        protected IConfigurationRoot Configuration
        {
            get => _configuration ?? LoadConfiguration();
            set => _configuration = value;
        }

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

        public TConfiguration GetSection<TConfiguration>(string sectionKey)
        {
            // load keys from custom user secrets file at 
            // %userprofile%/.secrets/AVS.Exchange.Core.Tests/secrets.json
            return Configuration.GetSection(sectionKey).Get<TConfiguration>();
        }

        /// <summary>
		/// load configuration from custom user secrets file at %userprofile%/.secrets/{AppName}/secrets.json 
		/// </summary>
        private IConfigurationRoot LoadConfiguration()
        {
            var attribute = this.GetType().GetCustomAttributes<ConfigurationAttribute>().FirstOrDefault();
            _configuration = attribute == null ? ConfigurationHelper.LoadConfiguration() : ConfigurationHelper.LoadConfiguration(attribute);

            return _configuration;
        }
    }


}