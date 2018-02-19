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
    //TODO : create a library and a sample application
    public class RaspberryPi : IRaspberryPi, IDisposable
    {
        private readonly ILogger<RaspberryPi> _logger;
        private readonly WoopsaServer _woopsaServer;
        private readonly IGpio _gpio;
        private readonly ICamera _camera;

        public RaspberryPi(ILogger<RaspberryPi> logger, IGpio gpio, ICamera camera)
        {
            _logger = logger;
            _woopsaServer = new WoopsaServer(this);
            _woopsaServer.Authenticator = new SimpleAuthenticator("Raspberry", 
                (sender, e) =>  e.IsAuthenticated = e.Username == "admin" && e.Password == "admin");

            _gpio = gpio;
            _camera = camera;
            _logger.LogDebug("Raspberry pi created");


            //TODO : to that more dynamically
            _woopsaServer.WebServer.Routes.Add("camera", HTTPMethod.GET, new RouteHandlerFileSystem(Camera.SharedFolder));
        }

        public IGpio Gpio => _gpio;  
        
        public ICamera Camera => _camera;     

        //TODO : Save on persistant
        public string SharedFolder { get; set; }

        #region IDisposable
        
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            _logger.LogDebug("Raspberry pi disposing...");
            
            if (!_disposedValue)
            {
                if (disposing)
                {
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
