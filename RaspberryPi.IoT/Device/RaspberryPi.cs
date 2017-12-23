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
        private readonly Gpio _gpio;

        public RaspberryPi(ILogger<RaspberryPi> logger)
        {
            _logger = logger;
            _gpio = new Gpio(logger);
            _woopsaServer = new WoopsaServer(this);
            _logger.LogDebug("Raspberry pi created");
        }

        public IGpio Gpio => _gpio;

        public void Dispose()
        {
            _gpio.Dispose();
            _woopsaServer.Dispose();
            _logger.LogDebug("Raspberry pi diposed");
        }
    }
}
