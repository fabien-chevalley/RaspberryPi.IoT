using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Threading;
using Woopsa;

namespace RaspberryPi.IoT
{
    class Program
    {
        private static IRaspberryPi _raspberryPi;

        static void Main(string[] args)
        {
            var servicesProvider = RegisterServices();
            _raspberryPi = servicesProvider.GetRequiredService<IRaspberryPi>();

            //var pin = raspberry.OpenPin(18);
            //pin.SetDriveMode(GpioPinModes.Output);
            //pin.Write(GpioPinValues.High);
            //Thread.Sleep(1000);
            //pin.Write(GpioPinValues.Low);
            //Thread.Sleep(1000);
            //pin.Write(GpioPinValues.High);
            //Thread.Sleep(1000);
            //pin.Write(GpioPinValues.Low);
            //Thread.Sleep(1000);
            //pin.Write(GpioPinValues.High);
            //Thread.Sleep(1000);
            //pin.Write(GpioPinValues.Low);
            //Thread.Sleep(1000);

            Console.WriteLine("Press ANY key to exit");
            Console.ReadLine();
            _raspberryPi.Dispose();

            //AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
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
