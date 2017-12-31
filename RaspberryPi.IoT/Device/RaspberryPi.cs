using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MMALSharp;
using Woopsa;
using MMALSharp.Handlers;
using MMALSharp.Components;
using MMALSharp.Native;
using Nito.AsyncEx;

namespace RaspberryPi.IoT
{
    public class RaspberryPi : IRaspberryPi, IDisposable
    {
        private readonly ILogger<RaspberryPi> _logger;
        private readonly WoopsaServer _woopsaServer;
        private readonly Gpio _gpio;
        private readonly Camera _camera;

        public RaspberryPi(ILogger<RaspberryPi> logger)
        {
            _logger = logger;
            _woopsaServer = new WoopsaServer(this);
            _gpio = new Gpio(logger);
            _camera = new Camera(logger);
            _logger.LogDebug("Raspberry pi created");
        }

        public IGpio Gpio => _gpio;  
        
        public Camera Camera => _camera;     

        #region IDisposable
        
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            _logger.LogDebug("Raspberry pi disposing...");
            
            if (!_disposedValue)
            {
                if (disposing)
                {
                    MMALCamera.Instance.Cleanup();
                    _gpio.Dispose();
                    _woopsaServer.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
