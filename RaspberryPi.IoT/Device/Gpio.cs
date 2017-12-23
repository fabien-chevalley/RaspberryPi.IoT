using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Woopsa;

namespace RaspberryPi.IoT
{
    public class Gpio : IGpio, IDisposable
    {
        private const string DevicePath = @"/sys/class/gpio";
        private readonly ILogger<RaspberryPi> _logger;
        private readonly List<GpioPin> _openPins;

        public Gpio(ILogger<RaspberryPi> logger)
        {
            _logger = logger;
            _openPins = new List<GpioPin>();
        }

        //TODO : Improve with pin status, not only pin number 
        public string Status => JsonSerializer.Serialize(_openPins.Select(p => p.PinNumber));

        public void Open(int pinNumber)
        {
            if (pinNumber < 1 || pinNumber > 26)
                throw new ArgumentOutOfRangeException("Valid pins are between 1 and 26.");

            var gpioDirectoryPath = Path.Combine(DevicePath, string.Concat("gpio", pinNumber.ToString()));
            var gpioExportPath = Path.Combine(DevicePath, "export");
            if (!Directory.Exists(gpioDirectoryPath))
            {
                File.WriteAllText(gpioExportPath, pinNumber.ToString());
                Directory.CreateDirectory(gpioDirectoryPath);
            }
            
            if(!_openPins.Any(p => p.PinNumber == pinNumber))
                _openPins.Add(new GpioPin(pinNumber, gpioDirectoryPath));
            
            _logger.LogDebug($"Open pin number {pinNumber}");
        }

        public void Close(int pinNumber)
        {
            if (pinNumber < 1 || pinNumber > 26)
                throw new ArgumentOutOfRangeException("Valid pins are between 1 and 26.");

            var gpioDirectoryPath = Path.Combine(DevicePath, string.Concat("gpio", pinNumber.ToString()));
            var gpioExportPath = Path.Combine(DevicePath, "unexport");
            if (Directory.Exists(gpioDirectoryPath))
                File.WriteAllText(gpioExportPath, pinNumber.ToString());
            
            _openPins.RemoveAll(p => p.PinNumber == pinNumber);
            _logger.LogDebug($"Close pin number {pinNumber}");
        }

        public void SetMode(int pinNumber, GpioModes mode)
        {
            var pin = _openPins.FirstOrDefault(p => p.PinNumber == pinNumber);
            if (pin == null)
                throw new ArgumentOutOfRangeException($"Pin {pinNumber} is not open");
            
            pin.SetMode(mode);
        }

        public void Write(int pinNumber, GpioValues value)
        {
            var pin = _openPins.FirstOrDefault(p => p.PinNumber == pinNumber);
            if (pin == null)
                throw new ArgumentOutOfRangeException($"Pin {pinNumber} is not open");

            pin.Write(value);
        }

        public GpioValues Read(int pinNumber)
        {
            var pin = _openPins.FirstOrDefault(p => p.PinNumber == pinNumber);
            if (pin == null)
                throw new ArgumentOutOfRangeException($"Pin {pinNumber} is not open");

            return pin.Read();
        }

        public void Dispose()
        {
            foreach(var pin in _openPins.ToArray())
                Close(pin.PinNumber);
        }
    }
}
