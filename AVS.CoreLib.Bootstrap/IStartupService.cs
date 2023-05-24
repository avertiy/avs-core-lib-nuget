namespace AVS.CoreLib.BootstrapTools
{
    /// <summary>
    /// For a convenience startup service is just an entry point like Program.Main
    /// <see cref="Bootstrap.Start{TStartupService}"/>
    /// <code>
    ///  Bootstrap.Start&lt;StartupService&gt;(services => {..register your services..})
    /// </code>
    /// <seealso cref="StartupServiceBase"/>
    /// </summary>
    public interface IStartupService
    {
        void Start();
    }

    public interface ITestService
    {
        void Test();
    }
}