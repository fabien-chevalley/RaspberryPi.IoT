using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Linq;

namespace RaspberryPi.IoT
{
    class Program
    {
        static void Main(string[] args)
        {
            var application = new Application();
            if (application.Start())
            {
                while (true)
                {
                    var command = Console.ReadLine();
                    if (command == "quit" || command == "exit" || command == "q")
                    {
                        application.Stop();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Command not recognized. List of commands :");
                        Console.WriteLine($"- quit (q) : Save and quit application");
                    }
                }
            }   
            else
            {
                Console.WriteLine("Unable to start application...");
                Console.WriteLine("Press a key to exit");
                Console.ReadLine();
            }
        }
    }

    public class Application 
    {
        private IRaspberryPi _raspberryPi;
        private ILogger _logger;

        public bool Start()
        {
            try
            {
                var servicesProvider = RegisterServices();
                _logger = servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Application");
                _logger.LogInformation("Application startup...");

                _raspberryPi = servicesProvider.GetRequiredService<IRaspberryPi>();
                
                var pin = _raspberryPi.Gpio.Pins.FirstOrDefault(p => p.PinNumber == 4);
                pin.Open();
                pin.SetMode(GpioModes.Output);
                pin.Write(GpioValues.High);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Stop()
        {
            _logger?.LogInformation("Application shutdown...");
            _raspberryPi?.Dispose();
        }

        private IServiceProvider RegisterServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Trace));
            services.AddSingleton<IRaspberryPi, RaspberryPi>();
            services.AddSingleton<IGpio, Gpio>();
            services.AddSingleton<ICamera, Camera>();

            var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            loggerFactory.ConfigureNLog("nlog.config");

            return serviceProvider;
        }
    }
}
