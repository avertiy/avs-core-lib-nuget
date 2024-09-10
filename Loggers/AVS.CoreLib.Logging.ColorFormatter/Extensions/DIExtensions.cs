namespace AVS.CoreLib.Logging.ColorFormatter.Extensions;

//public static class DIExtensions
//{
//    public static void AddConsoleColorLogging(this IServiceCollection services, IConfiguration configuration)
//    {
//        services.AddLogging(x =>
//        {
//            x.AddConfiguration(configuration.GetSection("Logging"));

//            x.AddConsoleWithColorFormatter(`options =>
//            {
//                options.IncludeScopes = true;
//                options.SingleLine = true;
//                options.TimestampFormat = "HH:mm:ss";
//                options.UseUtcTimestamp = true;
//                options.ScopeBehavior = ScopeBehavior.Header;
//                options.IncludeLogLevel = false;
//            });
//        });
//    }
//}