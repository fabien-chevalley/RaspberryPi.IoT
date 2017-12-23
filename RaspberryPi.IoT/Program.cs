using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;

namespace RaspberryPi.IoT
{
    class Program
    {
        private static IRaspberryPi _raspberryPi;

        static void Main(string[] args)
        {
            var servicesProvider = RegisterServices();
            _raspberryPi = servicesProvider.GetRequiredService<IRaspberryPi>();

            Console.WriteLine("Press ANY key to exit");
            Console.ReadLine();
            _raspberryPi.Dispose();
        }

        private static IServiceProvider RegisterServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Trace));
            services.AddSingleton<IRaspberryPi, RaspberryPi>();

            var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            loggerFactory.ConfigureNLog("nlog.config");

            return serviceProvider;
        }
    }
}
