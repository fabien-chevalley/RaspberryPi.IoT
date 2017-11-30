using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Woopsa;

namespace RaspberryPi.IoT
{
    public class RaspberryPi : IRaspberryPi, IDisposable
    {
        private const string DevicePath = @"/sys/class/gpio";
        private readonly ILogger<RaspberryPi> _logger;
        private readonly WoopsaServer _woopsaServer;

        public RaspberryPi(ILogger<RaspberryPi> logger)
        {
            _logger = logger;


            _logger.LogDebug("Woopsa server started...");
            _woopsaServer = new WoopsaServer(this);
        }

        public IDictionary<string, string> Pins
        {
            get
            {
                DirectoryInfo gpioPins = new DirectoryInfo(DevicePath);
                IDictionary<string, string> pinNameValues = new Dictionary<string, string>();

                var pinNames = gpioPins.GetDirectories()
                    .Where(m => m.Name.StartsWith("gpio"))
                    .Where(m => !m.Name.StartsWith("gpiochip"))
                    .Select(m => m.Name)
                    .ToArray();

                foreach (var pinName in pinNames)
                    pinNameValues.Add(pinName, File.ReadAllText(Path.Combine(DevicePath, pinName, "value")));

                return pinNameValues;
            }
        }

        public GpioPin OpenPin(int pinNumber)
        {
            if (pinNumber < 1 || pinNumber > 26)
                throw new ArgumentOutOfRangeException("Valid pins are between 1 and 26.");

            // add a file to the export directory with the name <<pin number>>
            // add folder under device path for "gpio<<pinNumber>>"
            var gpioDirectoryPath = Path.Combine(DevicePath, string.Concat("gpio", pinNumber.ToString()));
            var gpioExportPath = Path.Combine(DevicePath, "export");
            if (!Directory.Exists(gpioDirectoryPath))
            {
                File.WriteAllText(gpioExportPath, pinNumber.ToString());
                Directory.CreateDirectory(gpioDirectoryPath);
            }

            // instantiate the gpiopin object to return with the pin number.
            return new GpioPin(this, pinNumber, gpioDirectoryPath);
        }

        public void ClosePin(int pinNumber)
        {
            if (pinNumber < 1 || pinNumber > 26)
                throw new ArgumentOutOfRangeException("Valid pins are between 1 and 26.");

            // add a file to the export directory with the name <<pin number>>
            // add folder under device path for "gpio<<pinNumber>>"
            var gpioDirectoryPath = Path.Combine(DevicePath, string.Concat("gpio", pinNumber.ToString()));
            var gpioExportPath = Path.Combine(DevicePath, "unexport");
            if (Directory.Exists(gpioDirectoryPath))
                File.WriteAllText(gpioExportPath, pinNumber.ToString());
        }

        public void Dispose()
        {
            _logger.LogDebug("Woopsa server shutdown");
            _woopsaServer.Dispose();
        }

    }
}
